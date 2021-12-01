using System;
using System.Collections.Generic;
using System.Linq;
using Faultify.Report;
using Faultify.TestRunner.Shared;
using Faultify.TestRunner.TestRun;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using TestResult = Faultify.TestRunner.Shared.TestResult;

namespace Faultify.TestRunner
{
    public class TestProjectReportModelBuilder
    {
        private static readonly object Mutext = new();
        private readonly TestProjectReportModel _testProjectReportModel;

        public TestProjectReportModelBuilder(string testProjectName)
        {
            _testProjectReportModel = new TestProjectReportModel(testProjectName, TimeSpan.MaxValue);
        }

        public void AddTestResult(TestResults testResults, IEnumerable<MutationVariant> mutations,
            TimeSpan testRunDuration)
        {
            if (testResults == null) return;
            if (mutations == null) return;

            lock (Mutext)
            {
                foreach (var mutation in mutations)
                {
                    // if there is no mutation or if the mutation has already been added to the report, skip the entry
                    if (mutation?.Mutation == null ||
                        _testProjectReportModel.Mutations.Any(x =>
                            x.MutationId == mutation.MutationIdentifier.MutationId &&
                            x.MemberName == mutation.MutationIdentifier.MemberName))
                    {
                        continue;
                    }

                    // get all the tests that cover a mutation
                    var allTestsForMutation = mutation.MutationIdentifier.TestCoverage
                        .Select(testName => testResults.Tests.Find(t => t.Name == testName))
                        .ToList();

                    // if there are no tests covering the mutation, mark it with no coverage
                    // otherwise, determine the success based on the outcome of the tests
                    var mutationStatus = allTestsForMutation.Count == 0 ? MutationStatus.NoCoverage : GetMutationStatus(allTestsForMutation);

                    // Add mutation to the report
                    _testProjectReportModel.Mutations.Add(new MutationVariantReportModel(
                        mutation.Mutation.Report, "",
                        new MutationAnalyzerReportModel(mutation.MutationAnalyzerInfo.AnalyzerName,
                            mutation.MutationAnalyzerInfo.AnalyzerDescription),
                        mutationStatus,
                        testRunDuration,
                        mutation.OriginalSource,
                        mutation.MutatedSource,
                        mutation.MutationIdentifier.MutationId,
                        mutation.MutationIdentifier.MemberName
                    ));
                }
            }
        }

        public TestProjectReportModel Build(TimeSpan testDuration, int totalTestRuns)
        {
            _testProjectReportModel.InitializeMetrics(totalTestRuns, testDuration);
            return _testProjectReportModel;
        }

        private MutationStatus GetMutationStatus(List<TestResult> testResultsTests)
        {
            // if all tests have passed, the mutation wasn't caught (it survived)
            if (testResultsTests.All(t => t.Outcome == TestOutcome.Passed)) return MutationStatus.Survived;

            // if any of the tests have failed, the mutation was caught (it was killed)
            if (testResultsTests.Any(t => t.Outcome == TestOutcome.Failed)) return MutationStatus.Killed;

           // any other outcome is being marked as a timeout 
           return MutationStatus.Timeout;
        }
    }
}