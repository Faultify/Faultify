using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Faultify.TestRunner.Shared;
using Faultify.TestRunner.TestProcess;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NUnit.Engine.Runners;
using Engine = NUnit.Engine;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using TestResult = Faultify.TestRunner.Shared.TestResult;

namespace Faultify.TestRunner.NUnit
{
    public class NUnitTestHostRunnerFactory : ITestHostRunFactory
    {
        public ITestHostRunner CreateTestRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger logger)
        {
            return new NUnitTestHostRunner(testProjectAssemblyPath, timeout, logger);
        }
    }

    public class NUnitTestHostRunner : ITestHostRunner
    {
        private readonly string _testProjectAssemblyPath;
        private readonly TimeSpan _timeout;
        private readonly ILogger _logger;

        public NUnitTestHostRunner(string testProjectAssemblyPath, TimeSpan timeout, ILogger logger)
        {
            _testProjectAssemblyPath = testProjectAssemblyPath;
            _timeout = timeout;
            _logger = logger;
        }

        private Engine.TestFilter GetTestFilter(IEnumerable<string> tests)
        {
            var testFilterBuilder = new Engine.TestFilterBuilder();
            
            foreach (var test in tests)
            {
                testFilterBuilder.AddTest(test);
            }

            return testFilterBuilder.GetFilter();
        }

        public Task<TestResults> RunTests(TimeSpan timeout, IProgress<string> progress, IEnumerable<string> tests)
        {
            TestResults testResults = new TestResults();
            
            // Get an interface to the engine
            using Engine.ITestEngine engine = Engine.TestEngineActivator.CreateInstance();
            engine.WorkDirectory = new FileInfo(_testProjectAssemblyPath).DirectoryName;
            
            // Create a simple test package - one assembly, no special settings
            Engine.TestPackage package = new Engine.TestPackage(_testProjectAssemblyPath);
            package.AddSetting("DefaultTimeout", _timeout.Milliseconds);
            package.AddSetting("StopOnError", true);
            package.AddSetting("BaseDirectory", new FileInfo(_testProjectAssemblyPath).DirectoryName);
            
            // Get a runner for the test package
            using var runner = engine.GetRunner(package);

            try
            {
                XmlNode testSessionResult =
                    runner.Run(null, GetTestFilter(tests)); 

                var testCases = GetTestCases(testSessionResult);

                foreach (XmlNode node in testCases)
                {
                    if (node.Attributes != null)
                    {
                        var testName = node.Attributes.GetNamedItem("fullname").Value;
                        var testResult = node.Attributes.GetNamedItem("result").Value;
                        
                        testResults.Tests.Add(
                            new TestResult() { Name = testName, Outcome = ParseTestOutcome(testResult) });
                    }
                }
            }
            finally
            {
                runner.Dispose();
                engine.Dispose();
            }


            return Task.FromResult(testResults);
        }

        public Task<MutationCoverage> RunCodeCoverage(CancellationToken cancellationToken)
        {
            // Get an interface to the engine
            using Engine.ITestEngine engine = Engine.TestEngineActivator.CreateInstance();
            engine.WorkDirectory = new FileInfo(_testProjectAssemblyPath).DirectoryName;

            // Create a simple test package - one assembly, no special settings
            Engine.TestPackage package = new Engine.TestPackage(_testProjectAssemblyPath);

            package.AddSetting("DefaultTimeout", 1000);
            package.AddSetting("StopOnError", true);
            package.AddSetting("BaseDirectory", new FileInfo(_testProjectAssemblyPath).DirectoryName);

            // Get a runner for the test package
            using MasterTestRunner runner = (MasterTestRunner)engine.GetRunner(package);

            try
            {
                // Run all the tests in the assembly
                var testResult = runner.Run(null, Engine.TestFilter.Empty);
                var testCases = GetTestCases(testResult);

                HashSet<string> testNames = new HashSet<string>();

                foreach (XmlNode node in testCases)
                {
                    if (node.Attributes != null) testNames.Add(node.Attributes.GetNamedItem("fullname").Value);
                }

                // Read coverage that was registered by: `Faultify.Injection.CoverageRegistry.RegisterTestCoverage()`.
                var binary = File.ReadAllBytes(Path.Combine(
                    Assembly.GetExecutingAssembly().Location,
                    TestRunnerConstants.CoverageFileName));
                var mutationCoverage = MutationCoverage.Deserialize(binary);

                mutationCoverage.Coverage = mutationCoverage.Coverage
                    .Where(pair => testNames.Contains(pair.Key))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
                
                runner.StopRun(true);
                runner.Unload();

                return Task.FromResult(mutationCoverage);

            }
            catch (Exception e)
            {
                return Task.FromResult(new MutationCoverage()); 
            }
        }

        private TestOutcome ParseTestOutcome(string testOutcome)
        {
            switch (testOutcome)
            {
                default: return TestOutcome.Failed;
            }
        }

        private XmlNodeList GetTestCases(XmlNode xmlNode)
        {
            XmlDocument teamDoc = new XmlDocument();
            teamDoc.AppendChild(teamDoc.ImportNode(xmlNode.LastChild, true));
            var result = teamDoc.GetElementsByTagName("test-case");

            return result;
        }
    }
}