using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Faultify.TestRunner.Shared;

namespace Faultify.TestRunner.TestProcess
{
    /// <summary>
    ///     Runs the mutation test with 'dotnet test'.
    /// </summary>
    public class DotnetTestRunner
    {
        private readonly ProcessRunner _coverageProcessRunner;
        private readonly string _testAdapterPath;
        private readonly DirectoryInfo _testDirectoryInfo;

        private readonly FileInfo _testFileInfo;
        private readonly TimeSpan _timeout;

        public DotnetTestRunner(string testProjectAssemblyPath, TimeSpan timeout)
        {
            _testFileInfo = new FileInfo(testProjectAssemblyPath);
            _testDirectoryInfo = new DirectoryInfo(_testFileInfo.DirectoryName);

            _timeout = timeout;
            _testAdapterPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var coverageArguments = new DotnetTestArgumentBuilder(Path.GetFileName(testProjectAssemblyPath))
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
                CreateNoWindow = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = _testDirectoryInfo.FullName
            };

            _coverageProcessRunner = new ProcessRunner(coverageProcessStartInfo);
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

            var testProcessStartInfo = new ProcessStartInfo("dotnet ", testArguments)
            {

                WorkingDirectory = _testDirectoryInfo.FullName,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            return new ProcessRunner(testProcessStartInfo);
        }

        /// <summary>
        ///     Runs the given tests and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <param name="tests"></param>
        /// <returns></returns>
        public async Task<TestResults> RunTests(CancellationToken cancellationToken, IProgress<string> progress,
            IEnumerable<string> tests)
        {
            var testResultOutputPath = Path.Combine(_testDirectoryInfo.FullName, TestRunnerConstants.TestsFileName);

            var testResults = new List<TestResult>();
            var remainingTests = new HashSet<string>(tests);

            try
            {
                while (remainingTests.Any())
                {
                    var testProcessRunner = BuildTestProcessRunner(remainingTests);

                    await testProcessRunner.RunAsync(cancellationToken);

                    var testResultsBinary = await File.ReadAllBytesAsync(testResultOutputPath, cancellationToken);

                    var deserializedTestResults = TestResults.Deserialize(testResultsBinary);

                    remainingTests.RemoveWhere(x => deserializedTestResults.Tests.Any(y => y.Name == x));

                    foreach (var testResult in deserializedTestResults.Tests) testResults.Add(testResult);

                    
                }
            }
            finally
            {
                if (File.Exists(testResultOutputPath)) File.Delete(testResultOutputPath);
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
            var coverageOutputPath = Path.Combine(_testDirectoryInfo.FullName, TestRunnerConstants.CoverageFileName);

            try
            {
                var process = await _coverageProcessRunner.RunAsync(cancellationToken);

                if (process.ExitCode != 0) throw new ExitCodeException(process.ExitCode);

                var coverageBinary = await File.ReadAllBytesAsync(coverageOutputPath, cancellationToken);
                return MutationCoverage.Deserialize(coverageBinary);
            }
            finally
            {
                if (File.Exists(coverageOutputPath)) File.Delete(coverageOutputPath);
            }
        }
    }
}