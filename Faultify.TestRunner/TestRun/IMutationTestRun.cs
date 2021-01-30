using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Faultify.TestRunner.ProjectDuplication;
using Faultify.TestRunner.TestProcess;

namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    ///     Defines an interface for a test run that executes mutations and returns the test results.
    /// </summary>
    public interface IMutationTestRun
    {
        public int RunId { get; set; }

        public int MutationCount { get;}

        /// <summary>
        ///     Runs the mutation test and returns the test run results.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sessionProgressTracker"></param>
        /// <param name="dotnetTestRunner"></param>
        /// <param name="projectDuplication"></param>
        /// <returns></returns>
        Task<IEnumerable<TestRunResult>> RunMutationTestAsync(CancellationToken token,
            MutationSessionProgressTracker sessionProgressTracker, DotnetTestRunner dotnetTestRunner,
            TestProjectDuplication projectDuplication);


        /// <summary>
        ///     Initializes the mutation and filter out those who have had a time out.
        /// </summary>
        /// <param name="testProject"></param>
        /// <param name="timedOutMutationVariants"></param>
        void InitializeMutations(TestProjectDuplication testProject,
            IEnumerable<MutationVariantIdentifier> timedOutMutationVariants);
    }
}