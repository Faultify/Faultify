using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Faultify.Analyze;
using Faultify.Analyze.ConstantAnalyzer;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.OpcodeAnalyzer;
using Faultify.Report;
using Faultify.Report.HTMLReporter;
using Faultify.Report.PDFReporter;
using Faultify.TestRunner;
using Faultify.TestRunner.Dotnet;
using Faultify.TestRunner.Logging;
using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mono.Cecil.Cil;

namespace Faultify.Cli
{
    internal class Program
    {
        private static string _outputDirectory;
        private readonly ILoggerFactory _loggerFactory;
        
        public Program(
            IOptions<Settings> settings,
            ILoggerFactory loggerFactory
        )
        {
            _loggerFactory = loggerFactory;
        }

        private static async Task Main(string[] args)
        {
            var settings = ParseCommandlineArguments(args);

            var currentDate = DateTime.Now.ToString("yy-MM-dd");
            _outputDirectory = Path.Combine(settings.ReportPath, currentDate);
            Directory.CreateDirectory(_outputDirectory);

            var configurationRoot = BuildConfigurationRoot();
            var services = new ServiceCollection();
            services.Configure<Settings>(options => configurationRoot.GetSection("settings").Bind(options));

            services.AddLogging(c =>
            {
                c.SetMinimumLevel(LogLevel.Trace);

                c.AddFilter(LogConfiguration.TestHost, LogLevel.Trace);
                c.AddFilter(LogConfiguration.TestRunner, LogLevel.Trace);

                c.AddFile(o =>
                {
                    o.RootPath = _outputDirectory;
                    o.FileAccessMode = LogFileAccessMode.KeepOpenAndAutoFlush;

                    o.Files = new[]
                    {
                        new LogFileOptions
                        {
                            Path = "testhost-" + DateTime.Now.ToString("yy-MM-dd-H-mm") + ".log",
                            MinLevel = new Dictionary<string, LogLevel> {{LogConfiguration.TestHost, LogLevel.Trace}}
                        },
                        new LogFileOptions
                        {
                            Path = "testprocess-" + DateTime.Now.ToString("yy-MM-dd-H-mm") + ".log",
                            MinLevel = new Dictionary<string, LogLevel> {{LogConfiguration.TestRunner, LogLevel.Trace}}
                        }
                    };
                });
            });

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

            var cursorPosition = (0, 0);

            var progress = new Progress<MutationRunProgress>();
            progress.ProgressChanged += (sender, s) =>
            {
                if (s.LogMessageType == LogMessageType.TestRunUpdate)
                {
                    if (cursorPosition.Item1 == 0 && cursorPosition.Item2 == 0)
                        cursorPosition = (Console.CursorLeft, Console.CursorTop);

                    Console.SetCursorPosition(cursorPosition.Item1, cursorPosition.Item2);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n> [{s.Progress}%] {s.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (s.LogMessageType != LogMessageType.Other)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n> [{s.Progress}%] {s.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"> [{s.Progress}%] {s.Message}");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            };

            var progressTracker = new MutationSessionProgressTracker(progress, _loggerFactory);

            var testResult = await RunMutationTest(settings, progressTracker);

            progressTracker.LogBeginReportBuilding(settings.ReportType, settings.ReportPath);
            await GenerateReport(testResult, settings);
            progressTracker.LogEndFaultify(settings.ReportPath);
            await Task.CompletedTask;
        }

        private async Task<TestProjectReportModel> RunMutationTest(Settings settings,
            MutationSessionProgressTracker progressTracker)
        {
            var mutationTestProject =
                new MutationTestProject(settings.TestProjectPath, settings.MutationLevel, settings.Parallel,
                    _loggerFactory, new DotnetTestHostRunnerFactory());

            return await mutationTestProject.Test(progressTracker, CancellationToken.None);
        }

        private async Task GenerateReport(TestProjectReportModel testResult, Settings settings)
        {
            if (string.IsNullOrEmpty(settings.ReportPath))
                settings.ReportPath = Directory.GetCurrentDirectory();

            var mprm = new MutationProjectReportModel();
            mprm.TestProjects.Add(testResult);

            var reporter = ReportFactory(settings.ReportType);
            var reportBytes = await reporter.CreateReportAsync(mprm);

            var reportFileName = DateTime.Now.ToString("yy-MM-dd-H-mm") + reporter.FileExtension;

            await File.WriteAllBytesAsync(Path.Combine(_outputDirectory, reportFileName), reportBytes);
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