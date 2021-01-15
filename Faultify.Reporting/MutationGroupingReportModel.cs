using System.Collections.Generic;

namespace Faultify.Reporting
{
    public class MutationGroupingReportModel
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public IList<MutationReportModel> MutationResults { get; set; } = new List<MutationReportModel>();

        public int MutationsSurvived { get; set; }
        public int MutationsKilled { get; set; }
        public int MutationsTimedOut { get; set; }
        public int MutationsNoCoverage { get; set; }
        public int TotalMutationsSurvivedMutation { get; set; }
        public int TotalMutationsKilledMutation { get; set; }
        public int TotalMutation { get; set; }

        public void MutationSurvivedAndKilled()
        {
            foreach (var mutation in MutationResults)
                switch (mutation.Status)
                {
                    case MutationReportModel.MutationStatus.Survived:
                        MutationsSurvived++;
                        break;
                    case MutationReportModel.MutationStatus.NoCoverage:
                        MutationsNoCoverage++;
                        break;
                    case MutationReportModel.MutationStatus.Killed:
                        MutationsKilled++;
                        break;
                    case MutationReportModel.MutationStatus.Timeout:
                        MutationsTimedOut++;
                        break;
                }

            TotalMutation = MutationsKilled + MutationsSurvived + MutationsTimedOut + MutationsNoCoverage;
            TotalMutationsKilledMutation = MutationsKilled + MutationsTimedOut;
            TotalMutationsSurvivedMutation = MutationsNoCoverage + MutationsSurvived;
        }
    }
}