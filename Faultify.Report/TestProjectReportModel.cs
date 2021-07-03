using System;
using System.Collections.Generic;

namespace Faultify.Report
{
    public class TestProjectReportModel
    {
        public TestProjectReportModel(string testProjectName, TimeSpan testSessionDuration)
        {
            TestProjectName = testProjectName;
            TestSessionDuration = testSessionDuration;
        }

        public IList<MutationVariantReportModel> Mutations { get; set; } = new List<MutationVariantReportModel>();
        public string TestProjectName { get; }
        public TimeSpan TestSessionDuration { get; private set; }

        public int MutationsSurvived { get; private set; }
        public int MutationsKilled { get; private set; }
        public int MutationsNoCoverage { get; private set; }
        public int MutationsTimedOut { get; private set; }

        public int TotalMutations { get; private set; }
        public int TotalTestRuns { get; private set; }

        public float ScorePercentage { get; set; }

        public void InitializeMetrics(int totalTestRuns, TimeSpan testSessionDuration)
        {
            TotalTestRuns = totalTestRuns;
            TestSessionDuration = testSessionDuration;

            foreach (MutationVariantReportModel mutation in Mutations)
            {
                switch (mutation.TestStatus)
                {
                    case MutationStatus.Survived:
                        MutationsSurvived++;
                        break;
                    case MutationStatus.NoCoverage:
                        MutationsNoCoverage++;
                        break;
                    case MutationStatus.Killed:
                        MutationsKilled++;
                        break;
                    case MutationStatus.Timeout:
                        MutationsTimedOut++;
                        break;
                }
            }

            TotalMutations = MutationsKilled + MutationsSurvived + MutationsTimedOut + MutationsNoCoverage;
            ScorePercentage = (int) (100.0 / TotalMutations * MutationsKilled);
        }
    }
}
