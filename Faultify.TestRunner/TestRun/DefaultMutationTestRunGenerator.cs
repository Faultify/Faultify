using System.Collections.Generic;
using System.IO;
using System.Linq;
using Faultify.Analyzers;
using Faultify.Analyzers.AssemblyMutator;
using Faultify.Core.ProjectAnalyzing;

namespace Faultify.TestRunner.TestRun
{
    public class DefaultMutationTestRunGenerator : IMutationTestRunGenerator
    {
        public IEnumerable<IMutationTestRun> GenerateMutationTestRuns(Dictionary<int, HashSet<string>> testsPerMethod,
            TestProjectInfo testProjectInfo, MutationLevel mutationLevel)
        {
            var allMutations = GetMutationsForCoverage(testsPerMethod, testProjectInfo, mutationLevel);
            var mutations = GetTestGroups(allMutations);

            return mutations.Select(x => new DefaultMutationTestRun
            {
                MutationCoverageInfos = x
            });
        }

        private static IEnumerable<IList<MutationVariant>> GetTestGroups(IList<MutationVariant> coverage)
        {
            // Get all MutationsInfo
            var allMutations = new List<MutationVariant>(coverage);

            while (allMutations.Count > 0)
            {
                // Mark all tests as free slots
                var testSlots = new HashSet<string>(coverage.SelectMany(x => x.TestCoverage));

                var mutationsForThisRun = new List<MutationVariant>();

                foreach (var mutationCoverageInfo in allMutations.ToArray())
                {
                    // If all tests of slot are free
                    if (testSlots.IsSupersetOf(mutationCoverageInfo.TestCoverage))
                    {
                        // Remove all free slots
                        foreach (var test in mutationCoverageInfo.TestCoverage) testSlots.Remove(test);

                        mutationsForThisRun.Add(mutationCoverageInfo);
                        allMutations.Remove(mutationCoverageInfo);
                    }

                    if (testSlots.Count == 0) break;
                }

                yield return mutationsForThisRun;
            }
        }

        private IList<MutationVariant> GetMutationsForCoverage(Dictionary<int, HashSet<string>> coverage,
            TestProjectInfo testProjectInfo, MutationLevel mutationLevel)
        {
            var allMutations = new List<MutationVariant>();

            var decompilers = new Dictionary<AssemblyMutator, ICodeDecompiler>();

            foreach (var dependencyAssembly in testProjectInfo.DependencyAssemblies)
            {
                var dependencyInjectionPath =
                    Path.Combine(
                        new DirectoryInfo(testProjectInfo.ProjectInfo.AssemblyPath).Parent.FullName,
                        new FileInfo(dependencyAssembly.Module.Name).Name
                    );
                decompilers.Add(dependencyAssembly, new CodeDecompiler(dependencyInjectionPath));
            }

            foreach (var assembly in testProjectInfo.DependencyAssemblies)
            foreach (var type in assembly.Types)
            foreach (var method in type.Methods)
                if (coverage.TryGetValue(method.IntHandle, out var tests))
                    foreach (var group in method.AllMutations(mutationLevel))
                    foreach (var mutation in group)
                    {
                        var originalSource = decompilers[assembly].Decompile(method.Handle);
                        allMutations.Add(new MutationVariant(mutation, tests, group, assembly, method)
                            {OriginalSource = originalSource});
                    }

            return allMutations;
        }
    }
}