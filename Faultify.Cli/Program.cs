﻿using System;
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
    internal class Program
    {
        private readonly ILogger<Program> _logger;
        private readonly IOptions<Settings> _settings;

        public Program(
            IOptions<Settings> settings,
            ILogger<Program> logger
        )
        {
            _settings = settings;
            _logger = logger;
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