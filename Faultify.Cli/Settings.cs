using System.IO;
using CommandLine;

namespace Faultify.Cli
{
    internal class Settings
    {
        [Option('p', "testProjectName", Required = true,
            HelpText = "The path pointing to the test project project file.")]
        public string TestProjectPath { get; set; }

        [Option('r', "reportPath", Required = false, HelpText = "The path were the report will be saved.")]
        public string ReportPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "FaultifyOutput");

        [Option('t', "reportType", Required = false, HelpText = "Type of report to be generated, options: 'pdf', 'html', 'json'")]
        public string ReportType { get; set; } = "json";
    }
}