using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Faultify.TestRunner.TestProcess;

namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    ///     Defines an interface for a test run that executes mutations and returns the test results.
    /// </summary>
    public interface IMutationTestRun
    {
        /// <summary>
        ///     Runs the mutation test and returns the test run results.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sessionProgressTracker"></param>
        /// <param name="dotnetTestRunner"></param>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        Task<IEnumerable<TestRunResult>> RunMutationTestAsync(CancellationToken token,
            MutationSessionProgressTracker sessionProgressTracker, DotnetTestRunner dotnetTestRunner,
            TestProjectInfo projectInfo);

        /// <summary>
        ///     Flags mutations as timedout such that they can be excluded from test run.
        /// </summary>
        /// <param name="timedOutMutationVariants"></param>
        void FlagTimedOutMutations(IEnumerable<MutationVariant> timedOutMutationVariants);
    }
}