namespace Faultify.Report
{
    public class MutationAnalyzerReportModel
    {
        public MutationAnalyzerReportModel(string name, string fullName)
        {
            Name = name;
            FullName = fullName;
        }

        public string Name { get; set; }
        public string FullName { get; set; }
    }
}