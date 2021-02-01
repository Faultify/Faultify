﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Faultify.Analyze;
using Faultify.Analyze.AssemblyMutator;
using Faultify.Analyze.Mutation;
using Faultify.Core.ProjectAnalyzing;
using Faultify.Injection;
using Faultify.Report;
using Faultify.TestRunner.ProjectDuplication;
using Faultify.TestRunner.Shared;
using Faultify.TestRunner.TestProcess;
using Faultify.TestRunner.TestRun;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace Faultify.TestRunner
{
    public class MutationTestProject
    {
        private readonly MutationLevel _mutationLevel;
        private readonly int _parallel;
        private readonly string _testProjectPath;
        private readonly ILogger _testHostLogger;

        public MutationTestProject(string testProjectPath, MutationLevel mutationLevel, int parallel, ILoggerFactory loggerFactoryFactory)
        {
            _testProjectPath = testProjectPath;
            _mutationLevel = mutationLevel;
            _parallel = parallel;
            _testHostLogger = loggerFactoryFactory.CreateLogger("Faultify.TestHost");
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
        public async Task<TestProjectReportModel> Test(MutationSessionProgressTracker progressTracker,
            CancellationToken cancellationToken = default)
        {
            // Build project
            progressTracker.LogBeginPreBuilding();
            var projectInfo = await BuildProject(progressTracker, _testProjectPath);
            progressTracker.LogEndPreBuilding();

            // Copy project N times
            progressTracker.LogBeginProjectDuplication(_parallel);
            var testProjectCopier =
                new TestProjectDuplicator(Directory.GetParent(projectInfo.AssemblyPath).FullName);
            var duplications = testProjectCopier.MakeInitialCopies(projectInfo, _parallel);

            // Begin code coverage on first project.
            var duplicationPool = new TestProjectDuplicationPool(duplications);
            var coverageProject = duplicationPool.TakeTestProject();
            var coverageProjectInfo = GetTestProjectInfo(coverageProject);

            // Measure the test coverage 
            progressTracker.LogBeginCoverage();
            InjectAssembliesWithCodeCoverage(coverageProjectInfo);

            var coverageTimer = new Stopwatch();
            coverageTimer.Start();
            var coverage = await RunCoverage(coverageProject.TestProjectFile.FullFilePath(), cancellationToken);
            coverageTimer.Stop();

            // Start test session.
            var testsPerMutation = GroupMutationsWithTests(coverage);
            return StartMutationTestSession(coverageProjectInfo, testsPerMutation, progressTracker,
                cancellationToken, coverageTimer.Elapsed, duplicationPool);
        }

        /// <summary>
        ///     Returns information about the test project.
        /// </summary>
        /// <returns></returns>
        private TestProjectInfo GetTestProjectInfo(TestProjectDuplication duplication)
        {
            // Read the test project into memory.
            var projectInfo = new TestProjectInfo
            {
                TestModule = ModuleDefinition.ReadModule(duplication.TestProjectFile.FullFilePath())
            };

            // Foreach project reference load it in memory as an 'assembly mutator'.
            foreach (var projectReferencePath in duplication.TestProjectReferences)
            {
                var loadProjectReferenceModel = new AssemblyMutator(projectReferencePath.FullFilePath());

                if (loadProjectReferenceModel.Types.Count > 0)
                    projectInfo.DependencyAssemblies.Add(loadProjectReferenceModel);
            }

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
        private void InjectAssembliesWithCodeCoverage(TestProjectInfo projectInfo)
        {
            TestCoverageInjector.Instance.InjectTestCoverage(projectInfo.TestModule);
            TestCoverageInjector.Instance.InjectModuleInit(projectInfo.TestModule);
            TestCoverageInjector.Instance.InjectAssemblyReferences(projectInfo.TestModule);

            using var ms = new MemoryStream();
            projectInfo.TestModule.Write(ms);
            projectInfo.TestModule.Dispose();

            File.WriteAllBytes(projectInfo.TestModule.FileName, ms.ToArray());

            foreach (var assembly in projectInfo.DependencyAssemblies)
            {
                var dependencyInjectionPath = assembly.Module.FileName;
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
            var dotnetTestRunner = new DotnetTestRunner(testAssemblyPath, TimeSpan.FromSeconds(12), _testHostLogger);
            return await dotnetTestRunner.RunCodeCoverage(cancellationToken);
        }

        /// <summary>
        ///     Groups methods with all tests which cover those methods.
        /// </summary>
        /// <param name="coverage"></param>
        /// <returns></returns>
        private Dictionary<RegisteredCoverage, HashSet<string>> GroupMutationsWithTests(MutationCoverage coverage)
        {
            // Group mutations with tests.
            var testsPerMutation = new Dictionary<RegisteredCoverage, HashSet<string>>();
            foreach (var (testName, mutationIds) in coverage.Coverage)
            foreach (var registeredCoverage in mutationIds)
            {
                if (!testsPerMutation.TryGetValue(registeredCoverage, out var testNames))
                {
                    testNames = new HashSet<string>();
                    testsPerMutation.Add(registeredCoverage, testNames);
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
        /// <param name="testProjectDuplicationPool"></param>
        /// <returns></returns>
        private TestProjectReportModel StartMutationTestSession(TestProjectInfo testProjectInfo,
            Dictionary<RegisteredCoverage, HashSet<string>> testsPerMutation,
            MutationSessionProgressTracker sessionProgressTracker,
            CancellationToken cancellationToken, TimeSpan coverageTestRunTime,
            TestProjectDuplicationPool testProjectDuplicationPool)
        {
            // Generate the mutation test runs for the mutation session.
            var defaultMutationTestRunGenerator = new DefaultMutationTestRunGenerator();
            var runs = defaultMutationTestRunGenerator.GenerateMutationTestRuns(testsPerMutation, testProjectInfo,
                _mutationLevel);

            // Double the time the code coverage took such that test runs have some time run their tests (needs to be in seconds).
            var maxTestDuration = TimeSpan.FromSeconds((coverageTestRunTime * 2).Seconds);

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
                var testProject = testProjectDuplicationPool.AcquireTestProject();
                
                try
                {
                    testRun.InitializeMutations(testProject, timedOutMutations);

                    var singRunsStopwatch = new Stopwatch();
                    singRunsStopwatch.Start();
                    
                    var dotnetTestRunner =
                        new DotnetTestRunner(testProject.TestProjectFile.FullFilePath(), maxTestDuration, _testHostLogger);

                    var results = await testRun.RunMutationTestAsync(cancellationToken, sessionProgressTracker,
                        dotnetTestRunner, testProject);

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
                        $"The test process encountered an unexpected error. Continuing without this test run. Please consider to submit an github issue. {e}", LogMessageType.Error);
                    failedRuns += 1;
                }
                finally
                {
                    lock(this) {
                        completedRuns += 1;
                        sessionProgressTracker.LogTestRunUpdate(completedRuns, totalRunsCount, failedRuns);
                    }
                    testProject.FreeTestProject();
                }
            }

            IEnumerable<Task> tasks = from testRun in mutationTestRuns select RunTestRun(testRun);
            
            Task.WaitAll(tasks.ToArray());
            allRunsStopwatch.Stop();

            var report = reportBuilder.Build(allRunsStopwatch.Elapsed, totalRunsCount);
            sessionProgressTracker.LogEndTestSession(allRunsStopwatch.Elapsed, completedRuns, mutationCount, report.ScorePercentage);

            return report;
        }
    }
}