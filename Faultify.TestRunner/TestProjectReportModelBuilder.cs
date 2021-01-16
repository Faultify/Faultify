using System;
using System.Collections.Generic;
using System.Linq;
using Faultify.Reporting;
using Faultify.TestRunner.Shared;
using Faultify.TestRunner.TestRun;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using TestResult = Faultify.TestRunner.Shared.TestResult;

namespace Faultify.TestRunner
{
    public class TestProjectReportModelBuilder
    {
        private readonly Dictionary<string, TestCaseReportModel> _testCases;


        private readonly Dictionary<TestCaseReportModel, Dictionary<string, MutationGroupingReportModel>>
            _testGroupings;

        private readonly TestProjectReportModel _testProjectReportModel;

        public TestProjectReportModelBuilder(string testProjectName)
        {
            _testProjectReportModel = new TestProjectReportModel {Name = testProjectName};
            _testCases = new Dictionary<string, TestCaseReportModel>();
            _testGroupings = new Dictionary<TestCaseReportModel, Dictionary<string, MutationGroupingReportModel>>();
        }

        public void AddTestResult(TestResults testResults, MutationVariant mutation, TimeSpan testRunDuration)
        {
            foreach (var testResult in testResults.Tests.Where(x => mutation.TestCoverage.Contains(x.Name)))
            {
                if (!_testCases.TryGetValue(testResult.Name,
                    out var testCaseReportModel))
                {
                    testCaseReportModel = new TestCaseReportModel {Name = testResult.Name};
                    _testCases.Add(testResult.Name, testCaseReportModel);
                    _testGroupings.Add(testCaseReportModel,
                        new Dictionary<string, MutationGroupingReportModel>());
                }

                var mutationStatus =
                    GetMutationStatus(testResult);
                var mutationGroupingReportModels =
                    _testGroupings[testCaseReportModel];

                if (!mutationGroupingReportModels.TryGetValue(mutation.ParentGroup.AnalyzerName,
                    out var mutationGroupingReportModel))
                {
                    mutationGroupingReportModel = new MutationGroupingReportModel
                    {
                        Name = mutation.ParentGroup.AnalyzerName,
                        FullName = mutation.ParentGroup.AnalyzerDescription,
                        MutationResults = new List<MutationReportModel>()
                    };
                    mutationGroupingReportModels.Add(mutation.ParentGroup.AnalyzerName,
                        mutationGroupingReportModel);
                }

                mutationGroupingReportModel.MutationResults.Add(new MutationReportModel
                {
                    MutatedSource = mutation.MutatedSource,
                    OriginalSource = mutation.OriginalSource,
                    Name = mutation.Mutation.ToString(),
                    Duration = TimeSpan.Zero,
                    Status = mutationStatus
                });
            }
        }

        public TestProjectReportModel Build(TimeSpan testDuration)
        {
            foreach (var (key, value) in _testGroupings) key.Mutations = value.Values.ToList();

            _testProjectReportModel.Tests = _testCases.Values.ToList();
            _testProjectReportModel.Duration = testDuration;

            return _testProjectReportModel;
        }

        private MutationReportModel.MutationStatus GetMutationStatus(TestResult testResultsTests)
        {
            if (testResultsTests.Outcome == TestOutcome.Failed) return MutationReportModel.MutationStatus.Killed;

            if (testResultsTests.Outcome == TestOutcome.Passed) return MutationReportModel.MutationStatus.Survived;

            return MutationReportModel.MutationStatus.Timeout;
        }
    }
}