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
        private readonly TestProjectReportModel _testProjectReportModel;

        public TestProjectReportModelBuilder(string testProjectName)
        {
            _testProjectReportModel = new TestProjectReportModel(testProjectName, TimeSpan.MaxValue);
        }

        public void AddTestResult(TestResults testResults, MutationVariant mutation, TimeSpan testRunDuration)
        {
            foreach (var testResult in testResults.Tests.Where(x => mutation.TestCoverage.Contains(x.Name)))
            {
                var mutationStatus = GetMutationStatus(testResult);

                _testProjectReportModel.Mutations.Add(new MutationVariantReportModel(
                    mutation.Mutation.ToString(), "",
                    new MutationAnalyzerReportModel(mutation.ParentGroup.AnalyzerName,
                        mutation.ParentGroup.AnalyzerDescription),
                    mutationStatus,
                    testRunDuration, 
                    mutation.OriginalSource,
                    mutation.MutatedSource
                ));
            }
        }

        public TestProjectReportModel Build(TimeSpan testDuration, int totalTestRuns)
        {
            _testProjectReportModel.InitializeMetrics( totalTestRuns, testDuration);
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