using System.IO;
using CommandLine;
using Faultify.Analyze;

namespace Faultify.Cli
{
    internal class Settings
    {
        [Option('p', "testProjectName", Required = true,
            HelpText = "The path pointing to the test project project file.")]
        public string TestProjectPath { get; set; }

        [Option('r', "reportPath", Required = false, HelpText = "The path were the report will be saved.")]
        public string ReportPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "FaultifyOutput");

        [Option('t', "reportType", Required = false, Default = "json" , HelpText = "Type of report to be generated, options: 'pdf', 'html', 'json'")]
        public string ReportType { get; set; } 

        [Option('l', "mutationLevel", Required = false, Default = MutationLevel.Detailed,  HelpText = "The mutation level indicating the test depth. ")]
        public MutationLevel MutationLevel { get; set; }
    }
}