using System;
using Microsoft.Extensions.Logging;

namespace Faultify.TestRunner.XUnit
{
    [Obsolete("Deprecated parent interface")]
    public class XUnitTestHostRunnerFactory : ITestHostRunFactory
    {
        public TestFramework TestFramework => TestFramework.XUnit;

        public ITestHostRunner CreateTestRunner(string testProjectAssemblyPath, TimeSpan timeout)
        {
            return new XUnitTestHostRunner(testProjectAssemblyPath, timeout);
        }
    }
}