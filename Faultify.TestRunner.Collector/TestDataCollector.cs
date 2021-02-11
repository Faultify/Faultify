using System;
using System.IO;
using System.Linq;
using System.Xml;
using Faultify.TestRunner.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using TestResult = Faultify.TestRunner.Shared.TestResult;

namespace Faultify.TestRunner.Collector
{
    /// <summary>
    ///     Collects test run results and flushes those to a file.
    /// </summary>
    [DataCollectorFriendlyName("TestDataCollector")]
    [DataCollectorTypeUri("my://test/datacollector")]
    public class TestDataCollector : DataCollector
    {
        private readonly TestResults _testResults = new TestResults();
        private DataCollectionLogger _logger;
        private DataCollectionEnvironmentContext context;

        public override void Initialize(
            XmlElement configurationElement,
            DataCollectionEvents events,
            DataCollectionSink dataSink,
            DataCollectionLogger logger,
            DataCollectionEnvironmentContext environmentContext)
        {
            _logger = logger;
            context = environmentContext;

            events.TestCaseEnd += EventsOnTestCaseEnd;
            events.TestCaseStart += EventsOnTestCaseStart;

            events.SessionEnd += EventsOnSessionEnd;
            events.SessionStart += EventsOnSessionStart;
        }

        private void EventsOnSessionStart(object sender, SessionStartEventArgs e)
        {
            _logger.LogWarning(context.SessionDataCollectionContext, "Test Session Started");
        }

        private void EventsOnSessionEnd(object sender, SessionEndEventArgs e)
        {
            try
            {
                var serialized = _testResults.Serialize();
                File.WriteAllBytes(TestRunnerConstants.TestsFileName, serialized);
            }
            catch (Exception ex)
            {
                _logger.LogError(context.SessionDataCollectionContext, "Test Session Exception: {ex}");
            }

            _logger.LogWarning(context.SessionDataCollectionContext, "Test Session Finished");
        }

        private void EventsOnTestCaseStart(object sender, TestCaseStartEventArgs e)
        {
            _logger.LogWarning(context.SessionDataCollectionContext, $"Test Case Start: {e.TestCaseName}");

            // Register this test because there is a possibility for the test host to crash before the end event. 
            _testResults.Tests.Add(new TestResult
                {Outcome = TestOutcome.None, Name = e.TestElement.FullyQualifiedName});
        }

        private void EventsOnTestCaseEnd(object sender, TestCaseEndEventArgs e)
        {
            _logger.LogWarning(context.SessionDataCollectionContext, $"Test Case End: {e.TestCaseName}");

            // Find the test and set the correct test outcome.
            var test = _testResults.Tests.FirstOrDefault(x => x.Name == e.TestElement.FullyQualifiedName);

            if (test == null)
            {
                _logger.LogError(context.SessionDataCollectionContext,
                    "Test case end event received but no test case start was recorded earlier.");
                return;
            }

            test.Outcome = e.TestOutcome;
        }
    }
}