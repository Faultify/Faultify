using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Faultify.Analyze;
using Faultify.TestRunner.Shared;
using NLog;

namespace Faultify.TestRunner.TestRun
{
    public class DefaultMutationTestRunGenerator : IMutationTestRunGenerator
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public IEnumerable<IMutationTestRun> GenerateMutationTestRuns(
            Dictionary<RegisteredCoverage, HashSet<string>> testsPerMethod,
            TestProjectInfo testProjectInfo, MutationLevel mutationLevel)
        {
            _logger.Info($"Generating mutation test runs");
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

        private class MutationBucket
        {
            public HashSet<string> Tests;
            public List<MutationVariantIdentifier> Mutations;

            public MutationBucket(MutationVariantIdentifier initialMutation)
            {
                Tests = new HashSet<string>(initialMutation.TestCoverage);
                Mutations = new List<MutationVariantIdentifier> { initialMutation };
            }

            public void AddMutation(MutationVariantIdentifier mutation)
            {
                Tests.Union(mutation.TestCoverage);
                Mutations.Add(mutation);
            }

            public bool IntersectsWith(HashSet<string> tests)
            {
                return !tests.AsParallel().Any(test => Tests.Contains(test));
            }
        }

        private static IEnumerable<IList<MutationVariantIdentifier>> GreedyCoverageAlgorithm(IList<MutationVariantIdentifier> mutationVariants)
        {
            var buckets = new List<MutationBucket>();
            var mutations = mutationVariants.OrderByDescending(mutation => mutation.TestCoverage.Count);

            foreach (MutationVariantIdentifier mutation in mutations)
            {
                // Attempt to add the mutation to a bucket
                bool wasInserted = false;
                foreach (MutationBucket bucket in buckets)
                {
                    if (!bucket.IntersectsWith(mutation.TestCoverage))
                    {
                        bucket.AddMutation(mutation);
                        wasInserted = true;
                        break;
                    }
                }

                // If it fails, make a new bucket
                if (!wasInserted)
                {
                    buckets.Add(new MutationBucket(mutation));
                }
            }
            return buckets.Select(bucket => bucket.Mutations);
        }

        private static IEnumerable<IList<MutationVariantIdentifier>> OptimalCoverageAlgorithm(IList<MutationVariantIdentifier> mutationVariants)
        {
            // Get all MutationsInfo
            var allMutations = new List<MutationVariantIdentifier>(mutationVariants);

            while (allMutations.Count > 0)
            {
                // Mark all tests as free slots
                var freeTests = new HashSet<string>(mutationVariants.SelectMany(x => x.TestCoverage));

                var mutationsForThisRun = new List<MutationVariantIdentifier>();

                foreach (MutationVariantIdentifier mutation in allMutations.ToArray())
                {
                    // If all tests of slot are free
                    if (freeTests.IsSupersetOf(mutation.TestCoverage))
                    {
                        // Remove all free slots
                        foreach (string test in mutation.TestCoverage) freeTests.Remove(test);

                        mutationsForThisRun.Add(mutation);
                        allMutations.Remove(mutation);
                    }

                    if (freeTests.Count == 0) break;
                }

                yield return mutationsForThisRun;
            }
        }

        /// <summary>
        /// Groups mutations into groups that can be run in parallel
        /// </summary>
        /// <param name="mutationVariants"></param>
        /// <returns></returns>
        private static IEnumerable<IList<MutationVariantIdentifier>> GetTestGroups(IList<MutationVariantIdentifier> mutationVariants)
        {
            _logger.Info($"Building mutation groups for test runs");

            if (mutationVariants.Count > 500) // Magic number, optimal run size not yet clear
            {
                // Faster but non-optimal
                return GreedyCoverageAlgorithm(mutationVariants);
            }
            else
            {
                // Very poor time scaling
                return OptimalCoverageAlgorithm(mutationVariants);
            }
        }

        private IList<MutationVariantIdentifier> GetMutationsForCoverage(
            Dictionary<RegisteredCoverage, HashSet<string>> coverage,
            TestProjectInfo testProjectInfo, MutationLevel mutationLevel)
        {
            _logger.Info($"Generating dummy mutations for test coverage");
            var allMutations = new List<MutationVariantIdentifier>();

            foreach (var assembly in testProjectInfo.DependencyAssemblies)
                foreach (var type in assembly.Types)
                    foreach (var method in type.Methods)
                    {
                        var methodMutationId = 0;
                        var registeredMutation = coverage.FirstOrDefault(x =>
                            x.Key.AssemblyName == assembly.Module.Assembly.Name.Name &&
                            x.Key.EntityHandle == method.IntHandle);
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