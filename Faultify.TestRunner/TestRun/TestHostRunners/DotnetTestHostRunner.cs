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
using Microsoft.Extensions.Logging;
using NLog;

namespace Faultify.TestRunner.TestRun.TestHostRunners
{
    /// <summary>
    ///     Runs the mutation test with 'dotnet test'.
    /// </summary>
    public class DotnetTestHostRunner : ITestHostRunner
    {
        private static readonly bool DisableOutput = true;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _testAdapterPath;
        private readonly DirectoryInfo _testDirectoryInfo;

        private readonly FileInfo _testFileInfo;
        private readonly TimeSpan _timeout;
        public TestFramework TestFramework => TestFramework.None;

        public DotnetTestHostRunner(string testProjectAssemblyPath, TimeSpan timeout)
        {
            _testFileInfo = new FileInfo(testProjectAssemblyPath);
            _testDirectoryInfo = new DirectoryInfo(_testFileInfo.DirectoryName);

            _timeout = timeout;
             
            _testAdapterPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        /// <summary>
        ///     Runs the given tests and returns the results.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="tests"></param>
        /// <returns></returns>
        public async Task<TestResults> RunTests(TimeSpan timeout, IProgress<string> progress,
            IEnumerable<string> tests)
        {
            string testResultOutputPath = Path.Combine(_testDirectoryInfo.FullName, TestRunnerConstants.TestsFileName);

            var testResults = new List<TestResult>();
            var remainingTests = new HashSet<string>(tests);

            while (remainingTests.Any())
            {
                try
                {
                    ProcessRunner testProcessRunner = BuildTestProcessRunner(remainingTests);

                    await testProcessRunner.RunAsync();
                    _logger.Debug(testProcessRunner.Output.ToString());
                    _logger.Error(testProcessRunner.Error.ToString());

                    byte[] testResultsBinary = await File.ReadAllBytesAsync(testResultOutputPath,
                        new CancellationTokenSource(timeout).Token);

                    TestResults deserializedTestResults = TestResults.Deserialize(testResultsBinary);

                    remainingTests.RemoveWhere(x => deserializedTestResults.Tests.Any(y => y.Name == x));

                    foreach (TestResult testResult in deserializedTestResults.Tests)
                    {
                        testResults.Add(testResult);
                    }
                }
                catch (FileNotFoundException)
                {
                    _logger.Error(
                        "The file 'test_results.bin' was not generated." +
                        "This implies that the test run can not be completed. " +
                        "Consider opening up an issue with the logs found in the output folder."
                    );
                }
                finally
                {
                    if (File.Exists(testResultOutputPath))
                    {
                        File.Delete(testResultOutputPath);
                    }
                }
            }

            return new TestResults {Tests = testResults};
        }

        /// <summary>
        ///     Run the code coverage process.
        ///     This process finds out which tests cover which mutations.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<MutationCoverage> RunCodeCoverage(CancellationToken cancellationToken)
        {
            try
            {
                ProcessRunner coverageProcessRunner = BuildCodeCoverageTestProcessRunner();
                Process process = await coverageProcessRunner.RunAsync();

                string output = coverageProcessRunner.Output.ToString();

                _logger.Debug(output);
                _logger.Error(coverageProcessRunner.Error.ToString());

                if (process.ExitCode != 0)
                {
                    throw new ExitCodeException(process.ExitCode);
                }

                return Utils.ReadMutationCoverageFile();
            }
            catch (FileNotFoundException)
            {
                _logger.Error(
                    "The file 'coverage.bin' was not generated." +
                    "This implies that the test run can not be completed. " +
                    "Consider opening up an issue with the logs found in the output folder."
                );
                return new MutationCoverage();
            }
        }

        /// <summary>
        ///     Constructs an instance of a <see cref="ProcessRunner" /> for the test process.
        /// </summary>
        /// <param name="tests"></param>
        /// <returns></returns>
        private ProcessRunner BuildTestProcessRunner(IEnumerable<string> tests)
        {
            var testArguments = new DotnetTestArgumentBuilder(_testFileInfo.Name)
                .Silent()
                .WithoutLogo()
                .WithTimeout(_timeout)
                .WithTestAdapter(_testAdapterPath)
                .WithCollector("TestDataCollector")
                .WithTests(tests)
                .DisableDump()
                .Build();

            var testProcessStartInfo = new ProcessStartInfo("dotnet", testArguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _testDirectoryInfo.FullName,
                RedirectStandardOutput = DisableOutput,
                RedirectStandardError = DisableOutput
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
            var coverageArguments = new DotnetTestArgumentBuilder(_testFileInfo.Name)
                .Silent()
                .WithoutLogo()
                .WithTimeout(_timeout)
                .WithTestAdapter(_testAdapterPath)
                .WithCollector("CoverageDataCollector")
                .DisableDump()
                .Build();

            var coverageProcessStartInfo = new ProcessStartInfo("dotnet", coverageArguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = DisableOutput,
                RedirectStandardError = DisableOutput,
                WorkingDirectory = _testDirectoryInfo.FullName
            };

            _logger.Debug($"Coverage test process arguments: {coverageArguments}");

            return new ProcessRunner(coverageProcessStartInfo);
        }
    }
}