using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze;
using Faultify.TestRunner.Shared;

namespace Faultify.TestRunner.TestRun
{
    public class DefaultMutationTestRunGenerator : IMutationTestRunGenerator
    {
        public IEnumerable<IMutationTestRun> GenerateMutationTestRuns(
            Dictionary<RegisteredCoverage, HashSet<string>> testsPerMethod,
            TestProjectInfo testProjectInfo, MutationLevel mutationLevel)
        {
            var mutationTestRuns = new List<IMutationTestRun>();

            var allMutations = GetMutationsForCoverage(testsPerMethod, testProjectInfo, mutationLevel);
            var mutationGroups = GetTestGroups(allMutations).ToArray();

            for (var i = 0; i < mutationGroups.Length; i++)
            {
                var mutationGroup = mutationGroups[i];

                mutationTestRuns.Add(new DefaultMutationTestRun
                {
                    MutationIdentifiers = mutationGroup,
                    RunId = i,
                    MutationLevel = mutationLevel
                });
            }

            return mutationTestRuns;
        }

        private static IEnumerable<IList<MutationVariantIdentifier>> GetTestGroups(
            IList<MutationVariantIdentifier> coverage)
        {
            // Get all MutationsInfo
            var allMutations = new List<MutationVariantIdentifier>(coverage);

            while (allMutations.Count > 0)
            {
                // Mark all tests as free slots
                var testSlots = new HashSet<string>(coverage.SelectMany(x => x.TestCoverage));

                var mutationsForThisRun = new List<MutationVariantIdentifier>();

                foreach (var mutationVariant in allMutations.ToArray())
                {
                    // If all tests of slot are free
                    if (testSlots.IsSupersetOf(mutationVariant.TestCoverage))
                    {
                        // Remove all free slots
                        foreach (var test in mutationVariant.TestCoverage) testSlots.Remove(test);

                        mutationsForThisRun.Add(mutationVariant);
                        allMutations.Remove(mutationVariant);
                    }

                    if (testSlots.Count == 0) break;
                }

                yield return mutationsForThisRun;
            }
        }

        private IList<MutationVariantIdentifier> GetMutationsForCoverage(
            Dictionary<RegisteredCoverage, HashSet<string>> coverage,
            TestProjectInfo testProjectInfo, MutationLevel mutationLevel)
        {
            var allMutations = new List<MutationVariantIdentifier>();

            foreach (var assembly in testProjectInfo.DependencyAssemblies)
            foreach (var type in assembly.Types)
            foreach (var method in type.Methods)
            {
                var methodMutationId = 0;
                var registeredMutation = coverage.FirstOrDefault(x =>
                    x.Key.AssemblyName == assembly.Module.Assembly.Name.Name && x.Key.EntityHandle == method.IntHandle);
                var mutationGroupId = 0;

                if (registeredMutation.Key != null)
                    foreach (var group in method.AllMutations(mutationLevel))
                    {
                        foreach (var mutation in group)
                        {
                            allMutations.Add(new MutationVariantIdentifier(registeredMutation.Value,
                                method.AssemblyQualifiedName,
                                methodMutationId, mutationGroupId));

                            methodMutationId++;
                        }

                        mutationGroupId += 1;
                    }
            }

            return allMutations;
        }
    }
}