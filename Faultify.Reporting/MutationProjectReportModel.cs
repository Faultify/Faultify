using System;
using System.Collections.Generic;

namespace Faultify.Reporting
{
    public class MutationProjectReportModel
    {
        public string Name { get; set; } = "MyMutationProject";
        public IList<TestProjectReportModel> TestProjects { get; set; } = new List<TestProjectReportModel>();
        public TimeSpan Duration { get; set; }
        public int TotalKilled { get; set; }
        public int TotalSurvived { get; set; }
        public int TotalNoCoverage { get; set; }
        public int TotalTimedOut { get; set; }
        public int TotalMutationsKilledRunResult { get; set; }
        public int TotalMutationsSurvivedRunResult { get; set; }
        public float Total { get; set; }
        public float Score { get; set; }
        public int ScoreAsInt { get; set; }
        public string ScoreString { get; set; }

        public void TotalKilledAndSurvived()
        {
            foreach (var tpr in TestProjects)
            {
                tpr.TestResultSurvivedAndKilled();
                TotalKilled += tpr.MutationsKilled;
                TotalSurvived += tpr.MutationsSurvived;
                TotalNoCoverage += tpr.MutationsNoCoverage;
                TotalTimedOut += tpr.MutationsTimedOut;
                TotalMutationsKilledRunResult += tpr.TotalMutationsKilledProject;
                TotalMutationsSurvivedRunResult += tpr.TotalMutationsSurvivedProject;
                Total += tpr.TotalProject;
            }

            float killed = TotalKilled + TotalTimedOut;
            Score = killed / Total * 100;
            ScoreAsInt = (int) Score;
            ScoreString = $"{(int) Score}%";
        }
    }
}