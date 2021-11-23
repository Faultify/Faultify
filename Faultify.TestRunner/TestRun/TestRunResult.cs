using System.Collections.Generic;
using System.Linq;
using Faultify.TestRunner.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using TestResult = Faultify.TestRunner.Shared.TestResult;

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
        public IEnumerable<MutationVariantIdentifier> GetTimedOutTests()
        {
            var nonResultTests = TestResults?
                                     .Tests
                                     .Where(x => x.Outcome == TestOutcome.None)
                                 ?? Enumerable.Empty<TestResult>();

            var timedOutTests = new List<MutationVariantIdentifier>();
            foreach (var nonResult in nonResultTests)
                timedOutTests.AddRange(Mutations.Where(x => x.MutationIdentifier.TestCoverage.Contains(nonResult.Name))
                    .Select(x => x.MutationIdentifier));

            return timedOutTests;
        }
    }
}