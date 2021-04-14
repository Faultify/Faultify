using System;
using System.Collections.Generic;
using System.Text;
using Faultify.TestRunner.TestRun.TestHostRunners;
namespace Faultify.TestRunner.TestRun
{
    /// <summary>
    /// Static factory class for creating testRunners
    /// </summary>
    static class TestHostRunnerFactory
    {
        /// <summary>
        /// Create and return an ITestHostRunner
        /// </summary>
        /// <param name="testAssemblyPath">The path of the tested project</param>
        /// <param name="timeOut">The timeout for the test hosts</param>
        /// <param name="testHost">Type of testHost to instantiate</param>
        /// <param name="testHostLogger">Logger class</param>
        /// <returns></returns>
        public static ITestHostRunner CreateTestRunner(string testAssemblyPath, TimeSpan timeOut, TestHost testHost)
        {
            // private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
            ITestHostRunner testRunner = testHost switch
            {
                TestHost.NUnit => new NUnitTestHostRunner(testAssemblyPath, timeOut),
                TestHost.XUnit => new XUnitTestHostRunner(testAssemblyPath),
                TestHost.MsTest => throw new Exception("not yet implemented"), //TODO: Implement MSTest
                TestHost.DotnetTest => new DotnetTestHostRunner(testAssemblyPath, timeOut),
                _ => throw new Exception("test host not found") //TODO: Probably bad practice
            };

            return testRunner;
        }
    }
}
