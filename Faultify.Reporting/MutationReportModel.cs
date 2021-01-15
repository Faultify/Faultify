using System;

namespace Faultify.Reporting
{
    public class MutationReportModel
    {
        public enum MutationStatus
        {
            Survived,
            Killed,
            Timeout,
            NoCoverage
        }

        public string Name { get; set; }
        public MutationStatus Status { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }

        public string OriginalSource { get; set; }
        public string MutatedSource { get; set; }
    }
}