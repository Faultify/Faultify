using System;
using Microsoft.Extensions.Logging;

namespace Faultify.TestRunner.NUnit
{
    [Obsolete("Deprecated parent interface")]
    public class NUnitTestHostRunnerFactory : ITestHostRunFactory
    {
        public TestFramework TestFramework => TestFramework.NUnit;

        public ITestHostRunner CreateTestRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger logger)
        {
            return new NUnitTestHostRunner(testProjectAssemblyPath, timeout, logger);
        }
    }
}