using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Faultify.TestRunner.Logging;
using Faultify.TestRunner.ProjectDuplication;
using Microsoft.Extensions.Logging;

namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    ///     Defines an interface for a test run that executes mutations and returns the test results.
    /// </summary>
    public interface IMutationTestRun
    {
        public int RunId { get; set; }

        public int MutationCount { get; }

        /// <summary>
        ///     Runs the mutation test and returns the test run results.
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="sessionProgressTracker"></param>
        /// <param name="testHostRunnerFactory"></param>
        /// <param name="projectDuplication"></param>
        /// <returns></returns>
        Task<IEnumerable<TestRunResult>> RunMutationTestAsync(TimeSpan timeout,
            MutationSessionProgressTracker sessionProgressTracker, TestHost testHost,
            TestProjectDuplication projectDuplication, NLog.ILogger logger);


        /// <summary>
        ///     Initializes the mutation and filter out those who have had a time out.
        /// </summary>
        /// <param name="testProject"></param>
        /// <param name="timedOutMutationVariants"></param>
        void InitializeMutations(TestProjectDuplication testProject,
            IEnumerable<MutationVariantIdentifier> timedOutMutationVariants);
    }
}