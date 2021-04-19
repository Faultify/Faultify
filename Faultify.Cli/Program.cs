using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Faultify.Report;
using Faultify.Report.HTMLReporter;
using Faultify.Report.PDFReporter;
using Faultify.TestRunner;
using Faultify.TestRunner.Logging;
using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Faultify.Cli
{
    internal class Program
    {
        private static string _outputDirectory;
        private readonly ILoggerFactory _loggerFactory;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public Program(
            IOptions<Settings> _,
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
            services.AddLogging(builder => BuildLogging(builder));
            services.AddSingleton<Program>();

            var serviceProvider = services.BuildServiceProvider();
            var program = serviceProvider.GetService<Program>();

            ConfigureNLog();

            await program.Run(settings);
        }

        /// <summary>
        /// Builds a custom logging implementation to be attached to the running program
        /// </summary>
        /// <param name="builder">ILoggingBuilder that will handle the creation of the logger.</param>
        private static void BuildLogging(ILoggingBuilder builder)
        {
            builder.SetMinimumLevel(LogLevel.Trace);

            builder.AddFilter(LogConfiguration.TestHost, LogLevel.Trace);
            builder.AddFilter(LogConfiguration.TestRunner, LogLevel.Trace);

            builder.AddFile(o =>
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
        }

        /// <summary>
        /// Sets up NLog configuration programmatically
        /// </summary>
        private static void ConfigureNLog()
        {
            string logPath = $"{DateTime.Now.ToString("yy.MM.dd-HH.mm.ss")}.log";
            string logFormat = "[${level:uppercase=true}] ${longdate} | ${logger} :: ${message}";

            // Clear existing log
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }

            // Initialize configuration
            var config = new NLog.Config.LoggingConfiguration();

            // File target configuration
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = logPath,
                Layout = logFormat
            };

            config.AddRule(
                minLevel: NLog.LogLevel.Trace,
                maxLevel: NLog.LogLevel.Fatal,
                target: logfile
            );

            // console target configuration
            var logconsole = new NLog.Targets.ColoredConsoleTarget("logconsole")
            {
                Layout = logFormat
            };

            config.AddRule(
                minLevel: NLog.LogLevel.Info,
                maxLevel: NLog.LogLevel.Fatal,
                target: logconsole
            );

            // Apply configuration
            NLog.LogManager.Configuration = config;
        }

        private static Settings ParseCommandlineArguments(string[] args)
        {
            var settings = new Settings();

            var result = Parser.Default.ParseArguments<Settings>(args)
                .WithParsed(o => { settings = o; });

            if (result.Tag == ParserResultType.NotParsed) Environment.Exit(0);

            return settings;
        }

        private void PrintProgress(MutationRunProgress progress)
        {
            if (progress.LogMessageType == LogMessageType.TestRunUpdate
            ||  progress.LogMessageType != LogMessageType.Other)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n> [{progress.Progress}%] {progress.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"> [{progress.Progress}%] {progress.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private async Task Run(Settings settings)
        {
            ConsoleMessage.PrintLogo();
            ConsoleMessage.PrintSettings(settings);

            if (!File.Exists(settings.TestProjectPath))
            {
                _logger.Fatal($"The file {settings.TestProjectPath} could not be found. Terminating Faultify.");
                Environment.Exit(2); // 0x2 ERROR_FILE_NOT_FOUND
            }

            var progress = new Progress<MutationRunProgress>();
            progress.ProgressChanged += (sender, progress) => PrintProgress(progress);

            var progressTracker = new MutationSessionProgressTracker(progress, _loggerFactory);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var testResult = await RunMutationTest(settings, progressTracker);
            stopWatch.Stop();
            Console.WriteLine("runtime of RunMutationTest(program.cs line 185): " + stopWatch.Elapsed);

            progressTracker.LogBeginReportBuilding(settings.ReportType, settings.ReportPath);
            await GenerateReport(testResult, settings);
            progressTracker.LogEndFaultify(settings.ReportPath);
            await Task.CompletedTask;
        }

        private async Task<TestProjectReportModel> RunMutationTest(Settings settings,
            MutationSessionProgressTracker progressTracker)
        {

            var mutationTestProject = new MutationTestProject(
                settings.TestProjectPath,
                settings.MutationLevel,
                settings.Parallel,
                _loggerFactory,
                settings.TestHost
            );

            return await mutationTestProject.Test(progressTracker, CancellationToken.None);
        }

        private async Task GenerateReport(TestProjectReportModel testResult, Settings settings)
        {
            if (string.IsNullOrEmpty(settings.ReportPath))
            {
                settings.ReportPath = Directory.GetCurrentDirectory();
            }

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
                "JSON" => new JsonReporter(),
                _ => throw new ArgumentException($"The argument \"{type}\" is not a valid file output type")
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