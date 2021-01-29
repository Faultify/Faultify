using System.Collections.Generic;
using Faultify.Analyze;
using Faultify.TestRunner.Shared;

namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    ///     Interface for defining a mutation test run generator that returns mutation test run instances for the mutation test
    ///     session.
    /// </summary>
    public interface IMutationTestRunGenerator
    {
        /// <summary>
        ///     Generates mutation test runs for the mutation test session.
        /// </summary>
        /// <param name="testsPerMethod"></param>
        /// <param name="testProjectInfo"></param>
        /// <returns></returns>
        public IEnumerable<IMutationTestRun> GenerateMutationTestRuns(
            Dictionary<RegisteredCoverage, HashSet<string>> testsPerMethod,
            TestProjectInfo testProjectInfo, MutationLevel mutationLevel);
    }
}