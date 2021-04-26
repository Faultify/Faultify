using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faultify.Analyze;
using Faultify.TestRunner.Logging;
using Faultify.TestRunner.ProjectDuplication;
using Faultify.TestRunner.Shared;

namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    ///     Executes the mutation test run on a test project.
    /// </summary>
    internal class DefaultMutationTestRun : IMutationTestRun
    {
        private IList<MutationVariant> _mutationVariants;

        public IList<MutationVariantIdentifier> MutationIdentifiers;
        public MutationLevel MutationLevel { get; set; }

        public int RunId { get; set; }
        public int MutationCount => MutationIdentifiers.Count;

        public async Task<IEnumerable<TestRunResult>> RunMutationTestAsync(
            TimeSpan timeout,
            MutationSessionProgressTracker sessionProgressTracker,
            TestHost testHost,
            TestProjectDuplication testProject
        )
        {
            ExecuteMutations(testProject);

            IEnumerable<string>? runningTests = _mutationVariants.Where(y => !y.CausesTimeOut)
                .SelectMany(x => x.MutationIdentifier.TestCoverage);

            ITestHostRunner? testRunner = TestHostRunnerFactory.CreateTestRunner(
                testProject.TestProjectFile.FullFilePath(), TimeSpan.FromSeconds(12),
                testHost);

            TestResults? testResults =
                await testRunner.RunTests(timeout, sessionProgressTracker, runningTests);

            //ResetMutations(testProject);

            return new List<TestRunResult>
            {
                new TestRunResult
                {
                    TestResults = testResults,
                    Mutations = _mutationVariants,
                },
            };
        }

        public void InitializeMutations(
            TestProjectDuplication testProject,
            IEnumerable<MutationVariantIdentifier> timedOutMutationVariants
        )
        {
            _mutationVariants = testProject.GetMutationVariants(MutationIdentifiers, MutationLevel);
            FlagTimedOutMutations(timedOutMutationVariants);
        }

        /// <summary>
        ///     Flags mutations that timed out.
        /// </summary>
        /// <param name="timedOutMutationVariants"></param>
        private void FlagTimedOutMutations(IEnumerable<MutationVariantIdentifier> timedOutMutationVariants)
        {
            foreach (MutationVariantIdentifier timedOut in timedOutMutationVariants)
            {
                IEnumerable<MutationVariant>? toRemoveMutations = _mutationVariants.Where(x =>
                    x.MutationIdentifier.MutationGroupId == timedOut.MutationGroupId
                    && x.MutationIdentifier.MemberName == timedOut.MemberName);

                foreach (var toRemoveMutation in toRemoveMutations) toRemoveMutation.CausesTimeOut = true;
            }
        }

        /// <summary>
        ///     Execute any possible mutation.
        /// </summary>
        /// <param name="testProject"></param>
        private void ExecuteMutations(TestProjectDuplication testProject)
        {
            foreach (var mutationVariant in _mutationVariants)
            {
                if (mutationVariant.CausesTimeOut)
                {
                    continue;
                }

                // Execute mutation and flush it to the files.
                mutationVariant.Mutation?.Mutate();
            }

            testProject.FlushMutations(_mutationVariants);
        }


        /// <summary>
        ///     Resets any mutation that was performed.
        ///     TODO: Might no longer be useful since duplications are no longer recycled
        /// </summary>
        /// <param name="testProject"></param>
        private void ResetMutations(TestProjectDuplication testProject)
        {
            foreach (var mutationVariant in _mutationVariants)
            {
                if (mutationVariant.CausesTimeOut)
                {
                    continue;
                }

                // Reset mutation and flush it to the files.
                mutationVariant.Mutation?.Reset();
            }

            testProject.FlushMutations(_mutationVariants);
        }
    }
}
