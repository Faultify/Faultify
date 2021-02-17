using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Faultify.MemoryTest.TestInformation;
using Faultify.TestRunner.Shared;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using TestResult = Faultify.TestRunner.Shared.TestResult;

namespace Faultify.TestRunner.XUnit
{
    public class XUnitTestHostRunner : ITestHostRunner
    {
        private readonly string _testProjectAssemblyPath;
        private readonly TestResults _testResults = new TestResults();
        private readonly HashSet<string> _coverageTests = new HashSet<string>();

        public XUnitTestHostRunner(string testProjectAssemblyPath, TimeSpan _0, ILogger _1)
        {
            _testProjectAssemblyPath = testProjectAssemblyPath;
        }
        
        public async Task<TestResults> RunTests(TimeSpan timeout, IProgress<string> progress, IEnumerable<string> tests)
        {
            var hashedTests = new HashSet<string>(tests);

            var xunitHostRunner = new MemoryTest.XUnit.XUnitTestHostRunner(_testProjectAssemblyPath);
            xunitHostRunner.TestEnd += OnTestEnd;

            await xunitHostRunner.RunTestsAsync(CancellationToken.None, hashedTests);

            return _testResults;
        }

        public async Task<MutationCoverage> RunCodeCoverage(CancellationToken cancellationToken)
        {
            var xunitHostRunner = new MemoryTest.XUnit.XUnitTestHostRunner(_testProjectAssemblyPath);
            xunitHostRunner.TestEnd += OnTestEndCoverage;

            await xunitHostRunner.RunTestsAsync(CancellationToken.None);

            return ReadCoverageFile();
        }

        private void OnTestEnd(object? sender, TestEnd e)
        {
            _testResults.Tests.Add(new TestResult() { Name = e.TestName, Outcome = ParseTestOutcome(e.TestOutcome)});
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
