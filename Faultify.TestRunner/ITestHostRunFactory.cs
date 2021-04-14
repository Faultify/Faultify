using System;
using Microsoft.Extensions.Logging;


namespace Faultify.TestRunner
{
    [Obsolete("There is only one factory class now, TestHostRunnerFactory", true)]
    /// <summary>
    ///     Implement this factory for the creation of an <see cref="ITestHostRunner" /> instance.
    /// </summary>
    public interface ITestHostRunFactory
    {
        public TestFramework TestFramework { get; }
        public ITestHostRunner CreateTestRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger logger);
    }
}