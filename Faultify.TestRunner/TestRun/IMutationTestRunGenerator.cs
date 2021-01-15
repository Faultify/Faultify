using System.Collections.Generic;

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
        public IEnumerable<IMutationTestRun> GenerateMutationTestRuns(Dictionary<int, HashSet<string>> testsPerMethod,
            TestProjectInfo testProjectInfo);
    }
}