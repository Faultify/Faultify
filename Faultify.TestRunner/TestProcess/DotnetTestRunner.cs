using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Faultify.TestRunner.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using TestResult = Faultify.TestRunner.Shared.TestResult;

namespace Faultify.TestRunner.TestProcess
{
    public class DotnetTestRunner
    {
        private readonly ProcessRunner _coverageProcessRunner;
        private readonly string _testAdapterPath;
        private readonly string _testProjectPath;

        private readonly string _workingDirectory;

        public DotnetTestRunner(string testProjectPath)
        {
            _testProjectPath = testProjectPath;
            _workingDirectory = Path.GetDirectoryName(testProjectPath);
            _testAdapterPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var testProjectName = Path.GetFileName(testProjectPath);

            var coverageArguments = new DotnetTestArgumentBuilder(testProjectName)
                .Silent()
                .WithoutLogo()
                .WithTimeout(TimeSpan.FromSeconds(5))
                .WithTestAdapter(_testAdapterPath)
                .WithCollector("CoverageDataCollector")
                .Build();

            var coverageProcessStartInfo = new ProcessStartInfo("dotnet", coverageArguments)
            {
                WorkingDirectory = _workingDirectory,
                RedirectStandardOutput = false,
            };

            _coverageProcessRunner = new ProcessRunner(coverageProcessStartInfo);
        }

        private ProcessRunner BuildTestProcessRunner(string testProjectName, IEnumerable<string> tests)
        {
            var testArguments = new DotnetTestArgumentBuilder(testProjectName)
                .Silent()
                .WithoutLogo()
                .WithTimeout(TimeSpan.FromSeconds(5)) // TODO: make dynamic based on initial test run.
                .WithTestAdapter(_testAdapterPath)
                .WithCollector("TestDataCollector")
                .WithTests(tests)
                .Build();

            var testProcessStartInfo = new ProcessStartInfo("dotnet", testArguments)
            {
                WorkingDirectory = _workingDirectory,
                RedirectStandardOutput = false,
            };

            return new ProcessRunner(testProcessStartInfo);
        }

        public async Task<TestResults> RunTests(CancellationToken cancellationToken, IProgress<string> progress,
            IEnumerable<string> tests)
        {
            var testResultOutputPath = Path.Combine(_workingDirectory, TestRunnerConstants.TestsFileName);

            var results = new List<TestResult>();
            var testsHasmap = new HashSet<string>(tests);

            try
            {
                while (testsHasmap.Any())
                {
                    var testProcessRunner = BuildTestProcessRunner(_testProjectPath, testsHasmap);

                    var t = await testProcessRunner.RunAsync(CancellationToken.None);

                    var testResultsBinary = await File.ReadAllBytesAsync(testResultOutputPath, cancellationToken);

                    var testResults = TestResults.Deserialize(testResultsBinary);

                    testsHasmap.RemoveWhere(x => testResults.Tests.Any(y => y.Name == x));

                    foreach (var testResult in testResults.Tests)
                    {
                        if (testResult.Outcome == TestOutcome.None)
                            progress.Report(
                                $"Test {testResult.Name} crashed, this test will be excluded in future runs. Rerunning the test session...");

                        results.Add(testResult);
                    }
                }
            }
            finally
            {
                if (File.Exists(testResultOutputPath)) File.Delete(testResultOutputPath);
            }

            return new TestResults {Tests = results};
        }

        public async Task<MutationCoverage> RunCodeCoverage(CancellationToken cancellationToken)
        {
            var coverageOutputPath = Path.Combine(_workingDirectory, TestRunnerConstants.CoverageFileName);

            try
            {
                var process = await _coverageProcessRunner.RunAsync(cancellationToken);

                if (process.ExitCode != 0) throw new ExitCodeException(process.ExitCode);

                var coverageBinary = await File.ReadAllBytesAsync(coverageOutputPath, cancellationToken);
                var coverage = MutationCoverage.Deserialize(coverageBinary);
                return coverage;
            }
            finally
            {
                if (File.Exists(coverageOutputPath)) File.Delete(coverageOutputPath);
            }
        }
    }
}