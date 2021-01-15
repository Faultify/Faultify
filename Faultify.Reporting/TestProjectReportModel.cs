using System;
using System.Collections.Generic;

namespace Faultify.Reporting
{
    public class TestProjectReportModel
    {
        public string Name { get; set; }
        public IList<TestCaseReportModel> Tests { get; set; } = new List<TestCaseReportModel>();
        public TimeSpan Duration { get; set; }

        public int MutationsSurvived { get; set; }
        public int MutationsKilled { get; set; }
        public int MutationsNoCoverage { get; set; }
        public int MutationsTimedOut { get; set; }
        public int TotalMutationsSurvivedProject { get; set; }
        public int TotalMutationsKilledProject { get; set; }
        public int TotalProject { get; set; }

        public void TestResultSurvivedAndKilled()
        {
            foreach (var tr in Tests)
            {
                tr.TestMutationSurvivedAndKilled();
                MutationsSurvived += tr.MutationsSurvived;
                MutationsKilled += tr.MutationsKilled;
                MutationsNoCoverage += tr.MutationsNoCoverage;
                MutationsTimedOut += tr.MutationsTimedOut;
                TotalMutationsSurvivedProject += tr.TotalMutationsSurvivedResult;
                TotalMutationsKilledProject += tr.TotalMutationsKilledResult;
                TotalProject += tr.TotalResult;
            }
        }
    }
}