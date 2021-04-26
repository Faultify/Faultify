using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Faultify.TestRunner.Shared;

namespace Faultify.TestRunner
{
    /// <summary>
    ///     Interface for running tests and code coverage on some test host.
    /// </summary>
    public interface ITestHostRunner
    {
        /// <summary>
        ///     Identifies what test framework is being used
        /// </summary>
        public TestFramework TestFramework { get; }

        /// <summary>
        ///     Runs the given tests and returns the results.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="progress"></param>
        /// <param name="tests"></param>
        /// <returns></returns>
        Task<TestResults> RunTests(
            TimeSpan timeout,
            IProgress<string> progress,
            IEnumerable<string> tests
        );

        /// <summary>
        ///     Run the code coverage process.
        ///     This process finds out which tests cover which mutations.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<MutationCoverage> RunCodeCoverage(CancellationToken cancellationToken);
    }
}
