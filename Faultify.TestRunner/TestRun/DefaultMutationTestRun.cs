﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Faultify.Analyze;
using Faultify.TestRunner.ProjectDuplication;
using Faultify.TestRunner.TestProcess;

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


        public async Task<IEnumerable<TestRunResult>> RunMutationTestAsync(CancellationToken token,
            MutationSessionProgressTracker sessionProgressTracker, DotnetTestRunner dotnetTestRunner,
            TestProjectDuplication testProject)
        {
            ExecuteMutations(testProject);

            var runningTests = _mutationVariants.Where(y => !y.CausesTimeOut)
                .SelectMany(x => x.MutationIdentifier.TestCoverage);

            var testResults =
                await dotnetTestRunner.RunTests(token, sessionProgressTracker, runningTests);

            ResetMutations(testProject);

            return new List<TestRunResult>
            {
                new TestRunResult
                {
                    TestResults = testResults,
                    Mutations = _mutationVariants
                }
            };
        }

        public void InitializeMutations(TestProjectDuplication testProject,
            IEnumerable<MutationVariantIdentifier> timedOutMutationVariants)
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
            foreach (var timedOut in timedOutMutationVariants)
            {
                var toRemoveMutations = _mutationVariants.Where(x =>
                    x.MutationIdentifier.MutationGroupId == timedOut.MutationGroupId &&
                    x.MutationIdentifier.MemberName == timedOut.MemberName);

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
                    continue;

                // Execute mutation and flush it to the files.
                mutationVariant.Mutation?.Mutate();
            }

            testProject.FlushMutations(_mutationVariants);
        }

        /// <summary>
        ///     Resets any mutation that was performed.
        /// </summary>
        /// <param name="testProject"></param>
        private void ResetMutations(TestProjectDuplication testProject)
        {
            foreach (var mutationVariant in _mutationVariants)
            {
                if (mutationVariant.CausesTimeOut)
                    continue;

                // Execute mutation and flush it to the files.
                mutationVariant.Mutation?.Reset();
            }

            testProject.FlushMutations(_mutationVariants);
        }
    }
}