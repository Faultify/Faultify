using System;
using System.IO;
using CommandLine;
using Faultify.Analyze;
using Faultify.TestRunner;

// Disable Non-nullable is uninitialized, this is handled by the CommandLine package
#pragma warning disable 8618

namespace Faultify.Cli
{
    internal class Settings
    {
        [Option('i', "inputProject", Required = true,
            HelpText = "The path pointing to the test project project file.")]
        public string TestProjectPath { get; set; }

        [Option('r', "reportPath", Required = false, HelpText = "The path were the report will be saved.")]
        public string ReportPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "FaultifyOutput");

        [Option('f', "reportFormat", Required = false, Default = "json",
            HelpText = "Type of report to be generated, options: 'pdf', 'html', 'json'")]
        public string ReportType { get; set; }

        [Option('p', "parallel", Required = false, Default = 1,
            HelpText = "Defines how many test sessions are ran at the same time.")]
        public int Parallel { get; set; }

        [Option('l', "mutationLevel", Required = false, Default = MutationLevel.Detailed,
            HelpText = "The mutation level indicating the test depth. ")]
        public MutationLevel MutationLevel { get; set; }

        [Option('t', "testHost", Required = false, Default = nameof(TestHost.DotnetTest),
            HelpText = "The name of the test host framework.")]
        public string TestHostName { get; set; }

        [Option('d', "timeOut", Required = false, Default = 0, HelpText = "Time out in seconds for the mutations")]
        public double Seconds { get; set; }

        public TimeSpan TimeOut => TimeSpan.FromSeconds(Seconds);

        public TestHost TestHost
        {
            get => Enum.Parse<TestHost>(TestHostName, true);
            set => TestHostName = value.ToString();
        }
    }
}
