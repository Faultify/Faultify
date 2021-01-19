using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Faultify.Analyze.AssemblyMutator;
using Faultify.Core.ProjectAnalyzing;
using Faultify.TestRunner.TestProcess;

namespace Faultify.TestRunner.TestRun
{
    internal class DefaultMutationTestRun : IMutationTestRun
    {
        public IList<MutationVariant> MutationCoverageInfos;

        public void FlagTimedOutMutations(IEnumerable<MutationVariant> timedOutMutationVariants)
        {
            foreach (var timedOut in timedOutMutationVariants)
            {
                var toRemoveMutations = MutationCoverageInfos.Where(x =>
                    x.ParentGroup == timedOut.ParentGroup && x.Method.Name == timedOut.Method.Name);

                foreach (var toRemoveMutation in toRemoveMutations) toRemoveMutation.CausesTimeOut = true;
            }
        }

        public async Task<IEnumerable<TestRunResult>> RunMutationTestAsync(CancellationToken token,
            MutationSessionProgressTracker sessionProgressTracker, DotnetTestRunner dotnetTestRunner,
            TestProjectInfo projectInfo)
        {
            foreach (var mutationCoverageInfo in MutationCoverageInfos)
            {
                if (mutationCoverageInfo.CausesTimeOut)
                    continue;

                sessionProgressTracker.Report(
                    $"Found mutant: {mutationCoverageInfo.Mutation} in {mutationCoverageInfo.Method.Name}");

                // Execute mutation and flush it to the files.
                ExecuteMutation(mutationCoverageInfo, projectInfo);
            }
            
            // Create decompilers
            var decompilers = new Dictionary<AssemblyMutator, CodeDecompiler>();
            foreach (var mutationVariant in MutationCoverageInfos)
                if (!decompilers.ContainsKey(mutationVariant.Assembly))
                {
                    var dependencyInjectionPath =
                        GetAssemblyPathInTestFolder(projectInfo, mutationVariant.Assembly.Module.Name);
                    decompilers.Add(mutationVariant.Assembly, new CodeDecompiler(dependencyInjectionPath));
                }

            // Add decompiled mutated source to MutationVariant
            foreach (var mutationCoverageInfo in MutationCoverageInfos)
                mutationCoverageInfo.MutatedSource = decompilers[mutationCoverageInfo.Assembly]
                    .Decompile(mutationCoverageInfo.Method.Handle);

            var runningTests = MutationCoverageInfos.Where(y => !y.CausesTimeOut).SelectMany(x => x.TestCoverage);

            var testResults =
                await dotnetTestRunner.RunTests(token, sessionProgressTracker, runningTests);

            return new List<TestRunResult>
            {
                new TestRunResult
                {
                    TestResults = testResults,
                    Mutations = MutationCoverageInfos
                }
            };
        }

        private void ExecuteMutation(MutationVariant mutation, TestProjectInfo testProjectInfo)
        {
            mutation.Mutation.Mutate();
            mutation.Assembly.Flush();
            var dependencyInjectionPath = GetAssemblyPathInTestFolder(testProjectInfo, mutation.Assembly.Module.Name);
            mutation.Assembly.Flush(dependencyInjectionPath);
        }

        private string GetAssemblyPathInTestFolder(TestProjectInfo projectInfo, string assemblyName)
        {
            return Path.Combine(
                new DirectoryInfo(projectInfo.ProjectInfo.AssemblyPath).Parent.FullName,
                new FileInfo(assemblyName).Name
            );
        }
    }
}