using System;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Faultify.Report;
using Faultify.Report.HTMLReporter;
using Faultify.Report.PDFReporter;
using Faultify.TestRunner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Faultify.Cli
{
    static class ConsoleMessage
    {
        private static string _logo = @"    
             ______            ____  _ ____     
            / ____/___ ___  __/ / /_(_) __/_  __
           / /_  / __ `/ / / / / __/ / /_/ / / / 
          / __/ / /_/ / /_/ / / /_/ / __/ /_/ / 
         / _/    \__,_/\__,_/_/\__/_/_/  \__,/ 
                                       /____/  
        ";

        public static void PrintLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(_logo);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintSettings(Settings settings)
        {
            string settingsString = 
            $"\n" +
            $"| Mutation Level: {settings.MutationLevel} \t\t \n" +
            $"| Test Runners: {settings.Parallel} \t\t \n" +
            $"| Report Path: { settings.ReportPath} \t\t \n" +
            $"| Report Type: { settings.ReportType} \t\t \n" +
            $"| Test Project Path: { settings.TestProjectPath} \t\t \n" +
            $"\n";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(settingsString);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    internal class Program
    {
        public Program(
            IOptions<Settings> settings,
            ILogger<Program> logger
        )
        {
        }

        private static async Task Main(string[] args)
        {
            var settings = ParseCommandlineArguments(args);

            var configurationRoot = BuildConfigurationRoot();
            var services = new ServiceCollection();
            services.Configure<Settings>(options => configurationRoot.GetSection("settings").Bind(options));
            services.AddLogging(c => c.AddConsole());
            services.AddSingleton<Program>();
            var serviceProvider = services.BuildServiceProvider();
            var program = serviceProvider.GetService<Program>();

            await program.Run(settings);
        }

        private static Settings ParseCommandlineArguments(string[] args)
        {
            var settings = new Settings();

            var result = Parser.Default.ParseArguments<Settings>(args)
                .WithParsed(o => { settings = o; });

            if (result.Tag == ParserResultType.NotParsed) Environment.Exit(0);

            return settings;
        }

        private async Task Run(Settings settings)
        {
            ConsoleMessage.PrintLogo();
            ConsoleMessage.PrintSettings(settings);

            if (!File.Exists(settings.TestProjectPath))
                throw new Exception($"Test project '{settings.TestProjectPath}' can not be found.");

            var testResult = await RunMutationTest(settings);
            await GenerateReport(testResult, settings);

            await Task.CompletedTask;
        }

        private async Task<TestProjectReportModel> RunMutationTest(Settings settings)
        {
            var progress = new Progress<MutationRunProgress>();
            progress.ProgressChanged += (sender, s) => { Console.WriteLine($"[{s.Progress}%] {s.Message}"); };

            var mutationTestProject =
                new MutationTestProject(settings.TestProjectPath, settings.MutationLevel, settings.Parallel);
            return await mutationTestProject.Test(progress, CancellationToken.None);
        }

        private async Task GenerateReport(TestProjectReportModel testResult, Settings settings)
        {
            if (string.IsNullOrEmpty(settings.ReportPath))
                settings.ReportPath = Directory.GetCurrentDirectory();

            var mprm = new MutationProjectReportModel();
            mprm.TestProjects.Add(testResult);

            var reporter = ReportFactory(settings.ReportType);
            var reportBytes = await reporter.CreateReportAsync(mprm);

            var outputPath = settings.ReportPath;
            var reportFileName = DateTime.Now.ToString("yy-MM-dd-H-mm") + reporter.FileExtension;
            Directory.CreateDirectory(outputPath);

            await File.WriteAllBytesAsync(Path.Combine(outputPath, reportFileName), reportBytes);
        }

        private IReporter ReportFactory(string type)
        {
            return type?.ToUpper() switch
            {
                "PDF" => new PdfReporter(),
                "HTML" => new HtmlReporter(),
                _ => new JsonReporter()
            };
        }

        private static IConfigurationRoot BuildConfigurationRoot()
        {
            var builder = new ConfigurationBuilder();
            builder.AddUserSecrets<Program>();
            var configurationRoot = builder.Build();
            return configurationRoot;
        }
    }
}