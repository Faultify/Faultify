﻿using System;
using Microsoft.Extensions.Logging;

namespace Faultify.TestRunner.XUnit
{
    public class XUnitTestHostRunnerFactory : ITestHostRunFactory
    {
        public TestFramework TestFramework => TestFramework.XUnit;

        public ITestHostRunner CreateTestRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger logger)
        {
            return new XUnitTestHostRunner(testProjectAssemblyPath, timeout, logger);
        }
    }
}