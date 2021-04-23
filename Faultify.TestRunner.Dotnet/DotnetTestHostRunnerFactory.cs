using System;
using Microsoft.Extensions.Logging;

namespace Faultify.TestRunner.Dotnet
{

    [Obsolete("Deprecated parent interface")]
    public class DotnetTestHostRunnerFactory : ITestHostRunFactory
    {
        public TestFramework TestFramework => TestFramework.None;

        public ITestHostRunner CreateTestRunner(string testProjectAssemblyPath, TimeSpan timeout)
        {
            return new DotnetTestHostRunner(testProjectAssemblyPath, timeout);
        }
    }
}