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
    public class XUnitTestHostRunnerFactory : ITestHostRunFactory
    {
        public TestFramework TestFramework => TestFramework.XUnit;

        public ITestHostRunner CreateTestRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger logger)
        {
            return new XUnitTestHostRunner(testProjectAssemblyPath, timeout, logger);
        }
    }

    public class XUnitTestHostRunner : ITestHostRunner
    {
        private readonly string _testProjectAssemblyPath;
        private readonly TestResults _testResults = new TestResults();
        private string _coveragePath;
        private readonly HashSet<string> _coverageTests = new HashSet<string>();

        public XUnitTestHostRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger logger)
        {
            _testProjectAssemblyPath = testProjectAssemblyPath;

            _coveragePath = Path.Combine(
                new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName,
                TestRunnerConstants.CoverageFileName);
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
            using var mmf = MemoryMappedFile.OpenExisting("CoverageFile");
            using var stream = mmf.CreateViewStream();
            using MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            var mutationCoverage = MutationCoverage.Deserialize(memoryStream.ToArray());

            mutationCoverage.Coverage = mutationCoverage.Coverage
                .Where(pair => _coverageTests.Contains(pair.Key))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            return mutationCoverage;
        }
    }
}
