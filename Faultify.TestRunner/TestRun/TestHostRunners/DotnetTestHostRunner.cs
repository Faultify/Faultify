using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Faultify.TestRunner.Shared;
using Faultify.TestRunner.TestProcess;
using NLog;

namespace Faultify.TestRunner.TestRun.TestHostRunners
{
    /// <summary>
    ///     Runs the mutation test with 'dotnet test'.
    /// </summary>
    public class DotnetTestHostRunner : ITestHostRunner
    {
        private const bool DisableOutput = true;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _testAdapterPath;
        private readonly DirectoryInfo _testDirectoryInfo;

        private readonly FileInfo _testFileInfo;
        private readonly TimeSpan _timeout;

        public DotnetTestHostRunner(string testProjectAssemblyPath, TimeSpan timeout)
        {
            _testFileInfo = new FileInfo(testProjectAssemblyPath);
            _testDirectoryInfo = new DirectoryInfo(_testFileInfo.DirectoryName ?? string.Empty);

            _timeout = timeout;

            _testAdapterPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        }

        public TestFramework TestFramework => TestFramework.None;

        /// <summary>
        ///     Runs the given tests and returns the results.
        /// </summary>
        /// <returns></returns>
        public async Task<TestResults> RunTests(
            TimeSpan timeout,
            IProgress<string> progress,
            IEnumerable<string> tests
        )
        {
            _logger.Info("Running tests");
            string testResultOutputPath = Path.Combine(_testDirectoryInfo.FullName, TestRunnerConstants.TestsFileName);

            List<TestResult>? testResults = new List<TestResult>();
            HashSet<string>? remainingTests = new HashSet<string>(tests);

            while (remainingTests.Any())
            {
                try
                {
                    ProcessRunner testProcessRunner = BuildTestProcessRunner(remainingTests);

                    await testProcessRunner.RunAsync();

                    byte[] testResultsBinary = await File.ReadAllBytesAsync(testResultOutputPath,
                        new CancellationTokenSource(timeout).Token);

                    TestResults deserializedTestResults = TestResults.Deserialize(testResultsBinary);

                    if (deserializedTestResults.Tests.Count == 0)
                        throw new Exception("Dotnet cannot find the target file");

                    remainingTests.RemoveWhere(x => deserializedTestResults.Tests.Any(y => y.Name == x));

                    foreach (TestResult testResult in deserializedTestResults.Tests)
                    {
                        testResults.Add(testResult);
                    }
                }
                catch (FileNotFoundException e)
                {
                    _logger.Fatal(e,
                        "The file 'test_results.bin' was not generated."
                        + "This implies that the test run can not be completed. "
                    );
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }
                finally
                {
                    if (File.Exists(testResultOutputPath))
                    {
                        File.Delete(testResultOutputPath);
                    }
                }
            }

            return new TestResults { Tests = testResults };
        }

        /// <summary>
        ///     Run the code coverage process.
        ///     This process finds out which tests cover which mutations.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MutationCoverage> RunCodeCoverage(CancellationToken cancellationToken)
        {
            _logger.Info("Running code coverage");
            try
            {
                ProcessRunner coverageProcessRunner = BuildCodeCoverageTestProcessRunner();
                Process process = await coverageProcessRunner.RunAsync();

                if (process.ExitCode != 0)
                {
                    throw new ExitCodeException(process.ExitCode);
                }

                return Utils.ReadMutationCoverageFile();
            }
            catch (FileNotFoundException e)
            {
                _logger.Fatal(e,
                    "The file 'coverage.bin' was not generated."
                    + "This implies that the test run can not be completed. ");
            }
            catch (ExitCodeException e)
            {
                _logger.Fatal(e, $"Subprocess terminated with error code {e.ExitCode}");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error running code coverage.");
            }

            return new MutationCoverage();
        }

        /// <summary>
        ///     Constructs an instance of a <see cref="ProcessRunner" /> for the test process.
        /// </summary>
        /// <param name="tests"></param>
        /// <returns></returns>
        private ProcessRunner BuildTestProcessRunner(IEnumerable<string> tests)
        {
            _logger.Info("Building test runner");
            string testArguments = new DotnetTestArgumentBuilder(_testFileInfo.Name)
                .Silent()
                .WithoutLogo()
                .WithTimeout(_timeout)
                .WithTestAdapter(_testAdapterPath)
                .WithCollector("TestDataCollector")
                .WithTests(tests)
                .DisableDump()
                .Build();

            ProcessStartInfo testProcessStartInfo = new ProcessStartInfo("dotnet", testArguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _testDirectoryInfo.FullName,
                RedirectStandardOutput = DisableOutput,
                RedirectStandardError = DisableOutput,
            };

            _logger.Debug($"Test process process arguments: {testArguments}");

            return new ProcessRunner(testProcessStartInfo);
        }

        /// <summary>
        ///     Constructs an instance of a <see cref="ProcessRunner" /> for the coverage test process.
        /// </summary>
        /// <returns></returns>
        private ProcessRunner BuildCodeCoverageTestProcessRunner()
        {
            _logger.Info("Building coverage test runner");
            string? coverageArguments = new DotnetTestArgumentBuilder(_testFileInfo.Name)
                .Silent()
                .WithoutLogo()
                .WithTimeout(_timeout)
                .WithTestAdapter(_testAdapterPath)
                .WithCollector("CoverageDataCollector")
                .DisableDump()
                .Build();

            ProcessStartInfo? coverageProcessStartInfo = new ProcessStartInfo("dotnet", coverageArguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = DisableOutput,
                RedirectStandardError = DisableOutput,
                WorkingDirectory = _testDirectoryInfo.FullName,
            };

            _logger.Debug($"Coverage test process arguments: {coverageArguments}");

            return new ProcessRunner(coverageProcessStartInfo);
        }
    }
}
