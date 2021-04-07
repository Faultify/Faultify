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
        private static readonly object Mutext = new object();
        private readonly TestProjectReportModel _testProjectReportModel;

        public TestProjectReportModelBuilder(string testProjectName)
        {
            _testProjectReportModel = new TestProjectReportModel(testProjectName, TimeSpan.MaxValue);
        }

        public void AddTestResult(TestResults testResults, IEnumerable<MutationVariant> mutations,
            TimeSpan testRunDuration)
        {
            lock (Mutext)
            {
                foreach (var testResult in testResults.Tests)
                {
                    MutationVariant mutation =
                        mutations.FirstOrDefault(x => x.MutationIdentifier.TestCoverage.Any(y => y == testResult.Name));

                    if (mutation?.Mutation == null)
                        continue;

                    var mutationStatus = GetMutationStatus(testResult);

                    if (!_testProjectReportModel.Mutations.Any(x =>
                        x.MutationId == mutation.MutationIdentifier.MutationId &&
                        mutation.MutationIdentifier.MemberName == x.MemberName))
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

        private MutationStatus GetMutationStatus(TestResult testResultsTests)
        {
            if (testResultsTests.Outcome == TestOutcome.Failed) return MutationStatus.Killed;
            if (testResultsTests.Outcome == TestOutcome.Passed) return MutationStatus.Survived;

            return MutationStatus.Timeout;
        }
    }
}