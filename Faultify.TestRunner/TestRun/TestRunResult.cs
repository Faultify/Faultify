using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    ///     Contains results of a mutation test run.
    /// </summary>
    public class TestRunResult
    {
        /// <summary>
        ///     The results of the mutation test.
        /// </summary>
        public TestResults TestResults { get; set; }

        /// <summary>
        ///     The mutations that were executed during the test run.
        /// </summary>
        public IEnumerable<MutationVariant> Mutations { get; set; }

        /// <summary>
        ///     Returns mutations that timed out during the test run.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MutationVariant> GetTimedOutTests()
        {
            var nonResultTests = TestResults.Tests.Where(x => x.Outcome == TestOutcome.None);

            var timedOutTests = new List<MutationVariant>();
            foreach (var nonResult in nonResultTests)
                timedOutTests.AddRange(Mutations.Where(x => x.TestCoverage.Contains(nonResult.Name)));

            return timedOutTests;
        }
    }
}