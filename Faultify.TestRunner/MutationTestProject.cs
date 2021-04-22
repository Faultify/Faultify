extern alias MC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Faultify.Analyze;
using Faultify.Analyze.AssemblyMutator;
using Faultify.Core.ProjectAnalyzing;
using Faultify.Injection;
using Faultify.Report;
using Faultify.TestRunner.Logging;
using Faultify.TestRunner.ProjectDuplication;
using Faultify.TestRunner.Shared;
using Faultify.TestRunner.TestRun;
using Faultify.TestRunner.TestRun.TestHostRunners;
using Microsoft.Extensions.Logging;
using MC.Mono.Cecil;
using Newtonsoft.Json.Linq;
using NLog;

namespace Faultify.TestRunner
{
    public class MutationTestProject
    {
        private readonly MutationLevel _mutationLevel;
        private readonly int _parallel;
        private readonly TestHost _testHost;
        private readonly string _testProjectPath;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public MutationTestProject(string testProjectPath, MutationLevel mutationLevel, int parallel,
            ILoggerFactory loggerFactoryFactory, TestHost testHost)
        {
            _testProjectPath = testProjectPath;
            _mutationLevel = mutationLevel;
            _parallel = parallel;
            _testHost = testHost;
        }

        /// <summary>
        ///     Executes the mutation test session for the given test project.
        ///     Algorithm:
        ///     1. Build project
        ///     2. Calculate for each test which mutations they cover.
        ///     3. Rebuild to remove injected code.
        ///     4. Run test session.
        ///     4a. Generate optimized test runs.
        ///     5. Build report.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TestProjectReportModel> Test(MutationSessionProgressTracker progressTracker,
            CancellationToken cancellationToken = default)
        {
            // Build project
            progressTracker.LogBeginPreBuilding();
            var projectInfo = await BuildProject(progressTracker, _testProjectPath);
            progressTracker.LogEndPreBuilding();

            // Copy project N times
            var testProjectCopier = new TestProjectDuplicator(Directory.GetParent(projectInfo.AssemblyPath).FullName);

            // This is for some reason necessary when running tests with Dotnet,
            // otherwise the coverage analysis breaks future clones.
            // TODO: Should be investigated further.
            var initialCopy = testProjectCopier.MakeInitialCopy(projectInfo);

            // Begin code coverage on first project.
            TestProjectDuplication coverageProject = testProjectCopier.MakeCopy(1);
            TestProjectInfo coverageProjectInfo = GetTestProjectInfo(coverageProject, projectInfo);

            // Measure the test coverage 
            progressTracker.LogBeginCoverage();

            // Rewrites assemblies
            // FIX: Breaks line numbers
            // To fix, we need to restore the initial state of the assemblies prior to performing mutation testing.
            PrepareAssembliesForCodeCoverage(coverageProjectInfo);

            var coverageTimer = new Stopwatch();
            coverageTimer.Start();
            MutationCoverage coverage = await RunCoverage(coverageProject.TestProjectFile.FullFilePath(), cancellationToken);
            coverageTimer.Stop();

            _logger.Info($"Collecting garbage");
            GC.Collect();
            GC.WaitForPendingFinalizers();

            _logger.Info($"Freeing test project");
            coverageProject.FreeTestProject();

            // Start test session.
            var testsPerMutation = GroupMutationsWithTests(coverage);
            return StartMutationTestSession(coverageProjectInfo, testsPerMutation, progressTracker,
                coverageTimer.Elapsed, testProjectCopier, _testHost);
        }

        /// <summary>
        ///     Returns information about the test project.
        /// </summary>
        /// <returns></returns>
        private TestProjectInfo GetTestProjectInfo(TestProjectDuplication duplication, IProjectInfo testProjectInfo)
        {
            var testFramework = GetTestFramework(testProjectInfo);

            // Read the test project into memory.
            var projectInfo = new TestProjectInfo
            {
                TestFramework = testFramework,
                TestModule = ModuleDefinition.ReadModule(duplication.TestProjectFile.FullFilePath())
            };

            // Foreach project reference load it in memory as an 'assembly mutator'.
            foreach (FileDuplication projectReferencePath in duplication.TestProjectReferences)
            {
                AssemblyMutator loadProjectReferenceModel = new AssemblyMutator(projectReferencePath.FullFilePath());

                if (loadProjectReferenceModel.Types.Count > 0)
                    projectInfo.DependencyAssemblies.Add(loadProjectReferenceModel);
            }

            return projectInfo;
        }

        private TestFramework GetTestFramework(IProjectInfo projectInfo)
        {
            string projectFile = File.ReadAllText(projectInfo.ProjectFilePath);

            if (Regex.Match(projectFile, "xunit").Captures.Any())
            {
                return TestFramework.XUnit;
            }
            else if (Regex.Match(projectFile, "nunit").Captures.Any())
            {
                return TestFramework.NUnit;
            }
            else if (Regex.Match(projectFile, "mstest").Captures.Any())
            {
                return TestFramework.MsTest;
            }
            else
            {
                return TestFramework.None;
            }
        }


        /// <summary>
        ///     Builds the project at the given project path.
        /// </summary>
        /// <param name="sessionProgressTracker"></param>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        private async Task<IProjectInfo> BuildProject(
            MutationSessionProgressTracker sessionProgressTracker,
            string projectPath
        )
        {
            var projectReader = new ProjectReader();
            return await projectReader.ReadProjectAsync(projectPath, sessionProgressTracker);
        }

        /// <summary>
        ///     Injects the test project and all mutation assemblies with coverage injection code.
        ///     This code that is injected will register calls to methods/tests.
        ///     Those calls determine what tests cover which methods.
        /// </summary>
        /// <param name="projectInfo"></param>
        private void PrepareAssembliesForCodeCoverage(TestProjectInfo projectInfo)
        {
            _logger.Info($"Preparing assemblies for code coverage");
            TestCoverageInjector.Instance.InjectTestCoverage(projectInfo.TestModule);
            TestCoverageInjector.Instance.InjectModuleInit(projectInfo.TestModule);
            TestCoverageInjector.Instance.InjectAssemblyReferences(projectInfo.TestModule);

            using var ms = new MemoryStream();
            projectInfo.TestModule.Write(ms);
            projectInfo.TestModule.Dispose();

            File.WriteAllBytes(projectInfo.TestModule.FileName, ms.ToArray());

            foreach (var assembly in projectInfo.DependencyAssemblies)
            {
                _logger.Trace($"Writing assembly {assembly.Module.FileName}");
                TestCoverageInjector.Instance.InjectAssemblyReferences(assembly.Module);
                TestCoverageInjector.Instance.InjectTargetCoverage(assembly.Module);
                assembly.Flush(); // SHAME ON YOU, SHAME
                assembly.Dispose();
            }

            if (projectInfo.TestFramework == TestFramework.XUnit)
            {
                DirectoryInfo testDirectory = new FileInfo(projectInfo.TestModule.FileName).Directory;
                string xunitConfigFileName = Path.Combine(testDirectory.FullName, "xunit.runner.json");
                JObject xunitCoverageSettings = JObject.FromObject(new { parallelizeTestCollections = false });
                if (!File.Exists(xunitConfigFileName))
                {
                    File.WriteAllText(xunitConfigFileName, xunitCoverageSettings.ToString());
                }
                else
                {
                    var originalJsonConfig = JObject.Parse(File.ReadAllText(xunitConfigFileName));
                    originalJsonConfig.Merge(xunitCoverageSettings);
                    File.WriteAllText(xunitConfigFileName, originalJsonConfig.ToString());
                }
            }
        }

        /// <summary>
        ///     Runs the coverage process.
        ///     This process will get all tests with the methods covered by that test.
        /// </summary>
        /// <param name="testAssemblyPath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<MutationCoverage> RunCoverage(string testAssemblyPath, CancellationToken cancellationToken)
        {
            _logger.Info($"Running coverage analysis");
            using var _file = Utils.CreateCoverageMemoryMappedFile();
            ITestHostRunner testRunner = null;
            try
            {
                testRunner = TestHostRunnerFactory.CreateTestRunner(testAssemblyPath, TimeSpan.FromSeconds(12), _testHost);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Unable to create Test Runner");
            }

            return await testRunner.RunCodeCoverage(cancellationToken);
        }

        /// <summary>
        ///     Groups methods with all tests which cover those methods.
        /// </summary>
        /// <param name="coverage"></param>
        /// <returns></returns>
        private Dictionary<RegisteredCoverage, HashSet<string>> GroupMutationsWithTests(MutationCoverage coverage)
        {
            _logger.Info($"Grouping mutations with registered tests");
            // Group mutations with tests.
            var testsPerMutation = new Dictionary<RegisteredCoverage, HashSet<string>>();
            foreach (var (testName, mutationIds) in coverage.Coverage)
            {
                foreach (var registeredCoverage in mutationIds)
                {
                    if (!testsPerMutation.TryGetValue(registeredCoverage, out var testNames))
                    {
                        testNames = new HashSet<string>();
                        testsPerMutation.Add(registeredCoverage, testNames);
                    }

                    testNames.Add(testName);
                }
            }

            return testsPerMutation;
        }

        /// <summary>
        ///     Starts the mutation test session and returns the report with results.
        /// </summary>
        /// <param name="testProjectInfo"></param>
        /// <param name="testsPerMutation"></param>
        /// <param name="sessionProgressTracker"></param>
        /// <param name="coverageTestRunTime"></param>
        /// <param name="testProjectDuplicationPool"></param>
        /// <returns></returns>
        private TestProjectReportModel StartMutationTestSession(
            TestProjectInfo testProjectInfo,
            Dictionary<RegisteredCoverage, HashSet<string>> testsPerMutation,
            MutationSessionProgressTracker sessionProgressTracker,
            TimeSpan coverageTestRunTime,
            TestProjectDuplicator testProjectDuplicator,
            TestHost testHost
        )
        {
            _logger.Info($"Starting mutation session");
            // Generate the mutation test runs for the mutation session.
            var defaultMutationTestRunGenerator = new DefaultMutationTestRunGenerator();
            var runs = defaultMutationTestRunGenerator.GenerateMutationTestRuns(testsPerMutation, testProjectInfo,
                _mutationLevel);
            // Double the time the code coverage took such that test runs have some time run their tests (needs to be in seconds).
            TimeSpan maxTestDuration = TimeSpan.FromSeconds((coverageTestRunTime * 2).Seconds);

            var reportBuilder = new TestProjectReportModelBuilder(testProjectInfo.TestModule.Name);
            

            var allRunsStopwatch = new Stopwatch();
            allRunsStopwatch.Start();

            var mutationTestRuns = runs.ToList();
            var totalRunsCount = mutationTestRuns.Count();
            var mutationCount = mutationTestRuns.Sum(x => x.MutationCount);
            var completedRuns = 0;
            var failedRuns = 0;

            sessionProgressTracker.LogBeginTestSession(totalRunsCount, mutationCount, maxTestDuration);

            // Stores timed out mutations which will be excluded from test runs if they occur. 
            // Timed out mutations will be removed because they can cause serious test delays.
            var timedOutMutations = new List<MutationVariantIdentifier>();

            async Task RunTestRun(IMutationTestRun testRun)
            {
                var testProject = testProjectDuplicator.MakeCopy(testRun.RunId + 2);

                try
                {
                    testRun.InitializeMutations(testProject, timedOutMutations);

                    var singRunsStopwatch = new Stopwatch();
                    singRunsStopwatch.Start();
                    var results = await testRun.RunMutationTestAsync(
                        maxTestDuration,
                        sessionProgressTracker,
                        testHost,
                        testProject);
                    if (results != null)
                    {
                        foreach (var testResult in results)
                        {

                            // Store the timed out mutations such that they can be excluded.
                            timedOutMutations.AddRange(testResult.GetTimedOutTests());

                            // For each mutation add it to the report builder.
                            reportBuilder.AddTestResult(testResult.TestResults, testResult.Mutations,
                                singRunsStopwatch.Elapsed);
                        }

                        singRunsStopwatch.Stop();
                        singRunsStopwatch.Reset();
                    }
                }
                catch (Exception e)
                {
                    sessionProgressTracker.Log(
                        $"The test process encountered an unexpected error. Continuing without this test run. Please consider to submit an github issue. {e}",
                        LogMessageType.Error);
                    failedRuns += 1;
                    _logger.Error(e, "The test process encountered an unexpected error.");
                }
                finally
                {
                    lock (this)
                    {
                        completedRuns += 1;

                        sessionProgressTracker.LogTestRunUpdate(completedRuns, totalRunsCount, failedRuns);
                    }

                    testProject.FreeTestProject(); //TODO: replace with deletion
                }
            }
            
            var tasks = from testRun in mutationTestRuns select RunTestRun(testRun);

            Task.WaitAll(tasks.ToArray());
            allRunsStopwatch.Stop();

            var report = reportBuilder.Build(allRunsStopwatch.Elapsed, totalRunsCount);
            sessionProgressTracker.LogEndTestSession(allRunsStopwatch.Elapsed, completedRuns, mutationCount,
                report.ScorePercentage);

            return report;
        }
    }
}