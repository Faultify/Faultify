using System.Collections.Generic;

namespace Faultify.Reporting
{
    public class TestCaseReportModel
    {
        public string Name { get; set; }
        public IList<MutationGroupingReportModel> Mutations { get; set; } = new List<MutationGroupingReportModel>();
        public int MutationsSurvived { get; set; }
        public int MutationsKilled { get; set; }
        public int MutationsTimedOut { get; set; }
        public int MutationsNoCoverage { get; set; }
        public int TotalMutationsSurvivedResult { get; set; }
        public int TotalMutationsKilledResult { get; set; }
        public int TotalResult { get; set; }

        public void TestMutationSurvivedAndKilled()
        {
            foreach (var mutation in Mutations)
            {
                mutation.MutationSurvivedAndKilled();
                MutationsSurvived += mutation.MutationsSurvived;
                MutationsKilled += mutation.MutationsKilled;
                MutationsNoCoverage += mutation.MutationsNoCoverage;
                MutationsTimedOut += mutation.MutationsTimedOut;
                TotalMutationsSurvivedResult += mutation.TotalMutationsSurvivedMutation;
                TotalMutationsKilledResult += mutation.TotalMutationsKilledMutation;
                TotalResult += mutation.TotalMutation;
            }
        }
    }
}