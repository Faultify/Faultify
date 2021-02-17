using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Faultify.MemoryTest.TestInformation;
using Faultify.TestRunner.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using TestResult = Faultify.TestRunner.Shared.TestResult;

namespace Faultify.TestRunner.NUnit
{
    public class NUnitTestHostRunner : ITestHostRunner
    {
        private readonly string _testProjectAssemblyPath;
        private readonly TimeSpan _timeout;
        private readonly TestResults _testResults = new TestResults();
        private readonly HashSet<string> _coverageTests = new HashSet<string>();

        public NUnitTestHostRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger _)
        {
            _testProjectAssemblyPath = testProjectAssemblyPath;
            _timeout = timeout;
        }

        public async Task<TestResults> RunTests(TimeSpan timeout, IProgress<string> progress, IEnumerable<string> tests)
        {
            var hashedTests = new HashSet<string>(tests);

            var nunitHostRunner = new MemoryTest.NUnit.NUnitTestHostRunner(_testProjectAssemblyPath);
            nunitHostRunner.Settings.Add("DefaultTimeout", _timeout.Milliseconds);
            nunitHostRunner.Settings.Add("StopOnError", true);
            nunitHostRunner.Settings.Add("BaseDirectory", new FileInfo(_testProjectAssemblyPath).DirectoryName);

            nunitHostRunner.TestEnd += OnTestEnd;

            await nunitHostRunner.RunTestsAsync(CancellationToken.None, hashedTests);

            return _testResults;
        }

        public async Task<MutationCoverage> RunCodeCoverage(CancellationToken cancellationToken)
        {
            var nunitHostRunner = new MemoryTest.NUnit.NUnitTestHostRunner(_testProjectAssemblyPath);
            nunitHostRunner.Settings.Add("DefaultTimeout", 1000);
            nunitHostRunner.Settings.Add("StopOnError", true);
            nunitHostRunner.Settings.Add("BaseDirectory", new FileInfo(_testProjectAssemblyPath).DirectoryName);
            
            nunitHostRunner.TestEnd += OnTestEndCoverage;

            await nunitHostRunner.RunTestsAsync(CancellationToken.None);

            return ReadCoverageFile();
        }
        
        private void OnTestEnd(object? sender, TestEnd e)
        {
            _testResults.Tests.Add(new TestResult() { Name = e.TestName, Outcome = ParseTestOutcome(e.TestOutcome) });
        }

        private void OnTestEndCoverage(object? sender, TestEnd e)
        {
            _coverageTests.Add(e.FullTestName);
        }

        private TestOutcome ParseTestOutcome(MemoryTest.TestOutcome outcome)
        {
            return outcome switch
            {
                MemoryTest.TestOutcome.Passed => TestOutcome.Passed,
                MemoryTest.TestOutcome.Failed => TestOutcome.Failed,
                MemoryTest.TestOutcome.Skipped => TestOutcome.Skipped,
                _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, null)
            };
        }

        private MutationCoverage ReadCoverageFile()
        {
            var mutationCoverage = Utils.ReadMutationCoverageFile();

            mutationCoverage.Coverage = mutationCoverage.Coverage
                .Where(pair => _coverageTests.Contains(pair.Key))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return mutationCoverage;
        }
    }
}