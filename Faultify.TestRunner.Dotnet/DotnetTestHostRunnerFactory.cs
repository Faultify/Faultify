using System;
using Microsoft.Extensions.Logging;

namespace Faultify.TestRunner.Dotnet
{
    public class DotnetTestHostRunnerFactory : ITestHostRunFactory
    {
        public TestFramework TestFramework => TestFramework.None;

        public ITestHostRunner CreateTestRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger logger)
        {
            return new DotnetTestHostRunner(testProjectAssemblyPath, timeout, logger);
        }
    }
}