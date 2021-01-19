using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Faultify.Analyze;
using Faultify.Analyze.AssemblyMutator;
using Faultify.Core.ProjectAnalyzing;
using Faultify.Injection;
using Faultify.Report;
using Faultify.TestRunner.Shared;
using Faultify.TestRunner.TestProcess;
using Faultify.TestRunner.TestRun;
using Mono.Cecil;

namespace Faultify.TestRunner
{
    public class MutationTestProject
    {
        private readonly string _testProjectPath;
        private readonly MutationLevel _mutationLevel;
        public MutationTestProject(string testProjectPath, MutationLevel mutationLevel)
        {
            _testProjectPath = testProjectPath;
            _mutationLevel = mutationLevel;
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
        /// <param name="progress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TestProjectReportModel> Test(IProgress<MutationRunProgress> progress,
            CancellationToken cancellationToken = default)
        {
            var progressTracker = new MutationSessionProgressTracker(progress);

            // Build project
            progressTracker.LogBeginPreBuilding();
            var testProjectInfo = await GetTestProjectInfo(progressTracker, cancellationToken);
            progressTracker.LogEndPreBuilding();

            // Measure the test coverage 
            progressTracker.LogBeginCoverage();
            InjectAssembliesWithCodeCoverage(testProjectInfo, progressTracker);
            testProjectInfo.Dispose();

            Stopwatch coverageTimer = new Stopwatch();
            coverageTimer.Start();
            var coverage = await RunCoverage(testProjectInfo.ProjectInfo.AssemblyPath, cancellationToken);
            coverageTimer.Stop();
            progressTracker.LogEndCoverage();

            // Clean injection code.
            progressTracker.LogBeginCleanBuilding();
            testProjectInfo = await GetTestProjectInfo(progressTracker, cancellationToken);
            progressTracker.LogEndCleanBuilding();

            // Start test session.
            var testsPerMutation = GroupMutationsWithTests(coverage);
            return await StartMutationTestSession(testProjectInfo, testsPerMutation, progressTracker,
                cancellationToken, coverageTimer.Elapsed);
        }

        /// <summary>
        ///     Returns information about the test project.
        /// </summary>
        /// <param name="sessionProgressTracker"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<TestProjectInfo> GetTestProjectInfo(MutationSessionProgressTracker sessionProgressTracker,
            CancellationToken cancellationToken)
        {
            var projectInfo = new TestProjectInfo();

            // Build test project.
            var testProjectInfo = await BuildProject(sessionProgressTracker, _testProjectPath);

            // Read the test project into memory.
            projectInfo.TestModule = ModuleDefinition.ReadModule(testProjectInfo.AssemblyPath);

            // Foreach project reference load it in memory as an 'assembly mutator'.
            foreach (var projectReferencePath in testProjectInfo.ProjectReferences)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var projectReferenceInfo = await BuildProject(sessionProgressTracker, projectReferencePath);

                var loadProjectReferenceModel = new AssemblyMutator(projectReferenceInfo.AssemblyPath);

                if (loadProjectReferenceModel.Types.Count > 0)
                    projectInfo.DependencyAssemblies.Add(loadProjectReferenceModel);
            }

            projectInfo.ProjectInfo = testProjectInfo;

            return projectInfo;
        }

        /// <summary>
        ///     Builds the project at the given project path.
        /// </summary>
        /// <param name="sessionProgressTracker"></param>
        /// <param name="projectPath"></param>
        /// <returns></returns>
        private async Task<IProjectInfo> BuildProject(MutationSessionProgressTracker sessionProgressTracker,
            string projectPath)
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
        /// <param name="sessionProgressTracker"></param>
        private void InjectAssembliesWithCodeCoverage(TestProjectInfo projectInfo,
            MutationSessionProgressTracker sessionProgressTracker)
        {
            TestCoverageInjector.Instance.InjectTestCoverage(projectInfo.TestModule);
            TestCoverageInjector.Instance.InjectModuleInit(projectInfo.TestModule);
            TestCoverageInjector.Instance.InjectAssemblyReferences(projectInfo.TestModule);

            using var ms = new MemoryStream();
            projectInfo.TestModule.Write(ms);
            projectInfo.TestModule.Dispose();

            File.WriteAllBytes(projectInfo.ProjectInfo.AssemblyPath, ms.ToArray());

            foreach (var assembly in projectInfo.DependencyAssemblies)
            {
                var dependencyInjectionPath =
                    Path.Combine(
                        new DirectoryInfo(projectInfo.ProjectInfo.AssemblyPath).Parent.FullName,
                        new FileInfo(assembly.Module.FileName).Name
                    );

                sessionProgressTracker.Report($"Inject coverage code {dependencyInjectionPath}");
                TestCoverageInjector.Instance.InjectAssemblyReferences(assembly.Module);
                TestCoverageInjector.Instance.InjectTargetCoverage(assembly.Module);
                assembly.Module.Write(dependencyInjectionPath);
                assembly.Module.Dispose();
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
            var dotnetTestRunner = new DotnetTestRunner(testAssemblyPath, TimeSpan.FromSeconds(12));
            return await dotnetTestRunner.RunCodeCoverage(cancellationToken);
        }

        /// <summary>
        ///     Groups methods with all tests which cover those methods.
        /// </summary>
        /// <param name="coverage"></param>
        /// <returns></returns>
        private Dictionary<int, HashSet<string>> GroupMutationsWithTests(MutationCoverage coverage)
        {
            // Group mutations with tests.
            var testsPerMutation = new Dictionary<int, HashSet<string>>();
            foreach (var (testName, mutationIds) in coverage.Coverage)
            foreach (var mutationId in mutationIds)
            {
                if (!testsPerMutation.TryGetValue(mutationId, out var testNames))
                {
                    testNames = new HashSet<string>();
                    testsPerMutation.Add(mutationId, testNames);
                }

                testNames.Add(testName);
            }

            return testsPerMutation;
        }

        /// <summary>
        ///     Starts the mutation test session and returns the report with results.
        /// </summary>
        /// <param name="testProjectInfo"></param>
        /// <param name="testsPerMutation"></param>
        /// <param name="sessionProgressTracker"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="coverageTestRunTime"></param>
        /// <returns></returns>
        private async Task<TestProjectReportModel> StartMutationTestSession(TestProjectInfo testProjectInfo,
            Dictionary<int, HashSet<string>> testsPerMutation, MutationSessionProgressTracker sessionProgressTracker,
            CancellationToken cancellationToken, TimeSpan coverageTestRunTime)
        {
            // Generate the mutation test runs for the mutation session.
            var defaultMutationTestRunGenerator = new DefaultMutationTestRunGenerator();
            var runs = defaultMutationTestRunGenerator.GenerateMutationTestRuns(testsPerMutation, testProjectInfo, _mutationLevel);

            // Double the time the code coverage took such that test runs have some time run their tests (needs to be in seconds).
            var maxTestDuration = TimeSpan.FromSeconds((coverageTestRunTime * 2).Seconds);

            var dotnetTestRunner = new DotnetTestRunner(testProjectInfo.ProjectInfo.AssemblyPath, maxTestDuration);
            var reportBuilder = new TestProjectReportModelBuilder(testProjectInfo.TestModule.Name);

            var allRunsStopwatch = new Stopwatch();
            allRunsStopwatch.Start();

            var totalRunsCount = runs.Count();
            var testRunsExecutedCount = 0;

            sessionProgressTracker.LogBeginTestSession(totalRunsCount);

            // Stores timed out mutations which will be excluded from test runs if they occur. 
            // Timed out mutations will be removed because they can cause serious test delays.
            var timedOutMutations = new List<MutationVariant>();

            // Loop trough all test runs.
            foreach (var testRun in runs)
            {
                testRun.FlagTimedOutMutations(timedOutMutations);

                var singRunsStopwatch = new Stopwatch();
                singRunsStopwatch.Start();

                sessionProgressTracker.LogBeginTestRun(testRunsExecutedCount, totalRunsCount);

                var results = await testRun.RunMutationTestAsync(cancellationToken, sessionProgressTracker,
                    dotnetTestRunner, testProjectInfo);

                if (results == null)
                    continue;

                foreach (var testResult in results)
                {
                    // Store the timed out mutations such that they can be excluded.
                    timedOutMutations.AddRange(testResult.GetTimedOutTests());

                    foreach (var mutation in testResult.Mutations)
                        // For each mutation add it to the report builder.
                        reportBuilder.AddTestResult(testResult.TestResults, mutation, singRunsStopwatch.Elapsed);
                }

                testRunsExecutedCount += 1;

                singRunsStopwatch.Stop();
                sessionProgressTracker.LogEndTestRun(testRunsExecutedCount, totalRunsCount, singRunsStopwatch.Elapsed);
                singRunsStopwatch.Reset();
            }

            sessionProgressTracker.LogEndTestSession(allRunsStopwatch.Elapsed);
            allRunsStopwatch.Stop();

            return reportBuilder.Build(allRunsStopwatch.Elapsed);
        }
    }
}