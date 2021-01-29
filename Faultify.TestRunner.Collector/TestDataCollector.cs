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

        public override void Initialize(
            XmlElement configurationElement,
            DataCollectionEvents events,
            DataCollectionSink dataSink,
            DataCollectionLogger logger,
            DataCollectionEnvironmentContext environmentContext)
        {
            events.TestCaseEnd += EventsOnTestCaseEnd;
            events.SessionEnd += EventsOnSessionEnd;
            events.TestCaseStart += EventsOnTestCaseStart;
        }

        private void EventsOnSessionEnd(object sender, SessionEndEventArgs e)
        {
            try
            {
                var serialized = _testResults.Serialize();
                File.AppendAllText("debug.txt", $"\n\n file name: {TestRunnerConstants.TestsFileName}");
                File.WriteAllBytes(TestRunnerConstants.TestsFileName, serialized);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void EventsOnTestCaseStart(object sender, TestCaseStartEventArgs e)
        {
            File.AppendAllText("debug.txt", $"\n\n test case start {e.TestElement.DisplayName}");
            try
            {
                // Register this test because there is a possibility for the test host to crash before the end event. 
                _testResults.Tests.Add(new TestResult
                    {Outcome = TestOutcome.None, Name = e.TestElement.FullyQualifiedName});
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void EventsOnTestCaseEnd(object sender, TestCaseEndEventArgs e)
        {
            File.AppendAllText("debug.txt", $"\n\n test case end {e.TestElement.DisplayName}: {e.TestOutcome.ToString()} {e.TestElement.Source}");
            
            try
            {
                // Find the test and set the correct test outcome.
                var test = _testResults.Tests.First(x => x.Name == e.TestElement.FullyQualifiedName);
                test.Outcome = e.TestOutcome;
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}