using System;

namespace Faultify.Report
{
    public class MutationVariantReportModel
    {

        public MutationVariantReportModel(string name, string description, MutationAnalyzerReportModel mutationAnalyzer, MutationStatus testStatus, TimeSpan testDuration, string originalSource, string mutatedSource, int mutationId, string memberName)
        {
            Name = name;
            Description = description;
            MutationAnalyzer = mutationAnalyzer;
            TestStatus = testStatus;
            TestDuration = testDuration;
            OriginalSource = originalSource;
            MutatedSource = mutatedSource;
            MutationId = mutationId;
            MemberName = memberName;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public MutationAnalyzerReportModel MutationAnalyzer { get; set; }
        public MutationStatus TestStatus { get; set; }
        public TimeSpan TestDuration { get; set; }
        public string OriginalSource { get; set; }
        public string MutatedSource { get; set; }
        public int MutationId { get; set;  }
        public string MemberName { get; set; }
    }
}