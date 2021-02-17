using System.IO;
using CommandLine;
using Faultify.Analyze;

namespace Faultify.Cli
{
    public enum TestHost
    {
        NUnit,
        XUnit, 
        MsTest,
        DotnetTest
    }

    internal class Settings
    {
        [Option('t', "testProjectName", Required = true,
            HelpText = "The path pointing to the test project project file.")]
        public string TestProjectPath { get; set; }

        [Option('r', "reportPath", Required = false, HelpText = "The path were the report will be saved.")]
        public string ReportPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "FaultifyOutput");

        [Option('f', "reportFormat", Required = false, Default = "json",
            HelpText = "Type of report to be generated, options: 'pdf', 'html', 'json'")]
        public string ReportType { get; set; }

        [Option('p', "parallel", Required = false, Default = 1,
            HelpText = "Defines how many test sessions are ran at the same time. This option can only be used when using `dotnet test` testhost")]
        public int Parallel { get; set; }

        [Option('l', "mutationLevel", Required = false, Default = MutationLevel.Detailed,
            HelpText = "The mutation level indicating the test depth. ")]
        public MutationLevel MutationLevel { get; set; }

        [Option('h', "testHost", Required = false, Default = TestHost.DotnetTest,
            HelpText = "The test host that is used.")]
        public TestHost TestHost { get; set; }
    }
}