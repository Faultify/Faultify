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
using NLog;
using NLog.Config;
using NLog.Targets;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Faultify.Cli
{
    internal class Program
    {
        private static string _outputDirectory;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ILoggerFactory _loggerFactory;

        public Program(
            IOptions<Settings> _,
            ILoggerFactory loggerFactory
        )
        {
            _loggerFactory = loggerFactory;
        }

        private static async Task Main(string[] args)
        {
            Settings settings = ParseCommandlineArguments(args);

            var currentDate = DateTime.Now.ToString("yy-MM-dd");
            _outputDirectory = Path.Combine(settings.ReportPath, currentDate);

            Directory.CreateDirectory(_outputDirectory);

            IConfigurationRoot configurationRoot = BuildConfigurationRoot();

            ServiceCollection services = new ServiceCollection();
            services.Configure<Settings>(options => configurationRoot.GetSection("settings").Bind(options));
            services.AddLogging(builder => BuildLogging(builder));
            services.AddSingleton<Program>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            Program? program = serviceProvider.GetService<Program>();

            ConfigureNLog();

            await program.Run(settings);
        }

        /// <summary>
        ///     Builds a custom logging implementation to be attached to the running program
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
                        MinLevel = new Dictionary<string, LogLevel> { { LogConfiguration.TestHost, LogLevel.Trace } },
                    },
                    new LogFileOptions
                    {
                        Path = "testprocess-" + DateTime.Now.ToString("yy-MM-dd-H-mm") + ".log",
                        MinLevel = new Dictionary<string, LogLevel> { { LogConfiguration.TestRunner, LogLevel.Trace } },
                    },
                };
            });
        }

        /// <summary>
        ///     Sets up NLog configuration programmatically
        /// </summary>
        private static void ConfigureNLog()
        {
            var logPath = $"{DateTime.Now.ToString("yy.MM.dd-HH.mm.ss")}.log";
            var logFormat = "[${level:uppercase=true}] ${longdate} | ${logger} :: ${message}";

            // Clear existing log
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }

            // Initialize configuration
            LoggingConfiguration config = new LoggingConfiguration();

            // File target configuration
            FileTarget logfile = new FileTarget("logfile")
            {
                FileName = logPath,
                Layout = logFormat,
            };

            config.AddRule(
                NLog.LogLevel.Trace,
                NLog.LogLevel.Fatal,
                logfile
            );

            // console target configuration
            ColoredConsoleTarget logconsole = new ColoredConsoleTarget("logconsole")
            {
                Layout = logFormat,
            };

            config.AddRule(
                NLog.LogLevel.Info,
                NLog.LogLevel.Fatal,
                logconsole
            );

            // Apply configuration
            LogManager.Configuration = config;
        }

        private static Settings ParseCommandlineArguments(string[] args)
        {
            Settings settings = new Settings();

            ParserResult<Settings> result = Parser.Default.ParseArguments<Settings>(args)
                .WithParsed(o => { settings = o; });

            if (result.Tag == ParserResultType.NotParsed) Environment.Exit(0);

            return settings;
        }

        private void PrintProgress(MutationRunProgress progress)
        {
            if (progress.LogMessageType == LogMessageType.TestRunUpdate
                || progress.LogMessageType != LogMessageType.Other)
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

            Progress<MutationRunProgress> progress = new Progress<MutationRunProgress>();
            progress.ProgressChanged += (sender, progress) => PrintProgress(progress);

            MutationSessionProgressTracker progressTracker =
                new MutationSessionProgressTracker(progress, _loggerFactory);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            TestProjectReportModel testResult = await RunMutationTest(settings, progressTracker);
            stopWatch.Stop();
            Console.WriteLine("runtime of RunMutationTest(program.cs line 185): " + stopWatch.Elapsed);

            progressTracker.LogBeginReportBuilding(settings.ReportType, settings.ReportPath);
            await GenerateReport(testResult, settings);
            progressTracker.LogEndFaultify(settings.ReportPath);
            await Task.CompletedTask;
        }

        private async Task<TestProjectReportModel> RunMutationTest(
            Settings settings,
            MutationSessionProgressTracker progressTracker
        )
        {
            MutationTestProject mutationTestProject = new MutationTestProject(
                settings.TestProjectPath,
                settings.MutationLevel,
                settings.Parallel,
                _loggerFactory,
                settings.TestHost,
                settings.TimeOut
            );

            return await mutationTestProject.Test(progressTracker, CancellationToken.None);
        }

        private async Task GenerateReport(TestProjectReportModel testResult, Settings settings)
        {
            if (string.IsNullOrEmpty(settings.ReportPath))
            {
                settings.ReportPath = Directory.GetCurrentDirectory();
            }

            MutationProjectReportModel mprm = new MutationProjectReportModel();
            mprm.TestProjects.Add(testResult);

            IReporter reporter = ReportFactory(settings.ReportType);
            byte[] reportBytes = await reporter.CreateReportAsync(mprm);

            string reportFileName = DateTime.Now.ToString("yy-MM-dd-H-mm") + reporter.FileExtension;

            await File.WriteAllBytesAsync(Path.Combine(_outputDirectory, reportFileName), reportBytes);
        }

        private IReporter ReportFactory(string type)
        {
            return type?.ToUpper() switch
            {
                "PDF" => new PdfReporter(),
                "HTML" => new HtmlReporter(),
                "JSON" => new JsonReporter(),
                _ => throw new ArgumentException($"The argument \"{type}\" is not a valid file output type"),
            };
        }

        private static IConfigurationRoot BuildConfigurationRoot()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddUserSecrets<Program>();
            IConfigurationRoot configurationRoot = builder.Build();
            return configurationRoot;
        }
    }
}
