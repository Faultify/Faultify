using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Faultify.TestRunner.Shared;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace Faultify.TestRunner.Collector
{
    /// <summary>
    ///     Collects test methods that ran in a test session.
    ///     The collected methods will be used as filter for methods that were registered into
    ///     `Faultify.Injection.CoverageRegistry.RegisterTestCoverage()`/>
    /// </summary>
    [DataCollectorFriendlyName("CoverageDataCollector")]
    [DataCollectorTypeUri("my://coverage/datacollector")]
    public class CoverageDataCollector : DataCollector
    {
        /// <summary>
        ///     Collection with all tests that ran in the test session.
        /// </summary>
        private readonly HashSet<string> _testNames = new HashSet<string>();

        public override void Initialize(
            XmlElement configurationElement,
            DataCollectionEvents events,
            DataCollectionSink dataSink,
            DataCollectionLogger logger,
            DataCollectionEnvironmentContext environmentContext)
        {
            events.TestCaseEnd += EventsOnTestCaseEnd;
            events.SessionEnd += EventsOnSessionEnd;

            File.AppendAllText("debug.txt", $"\n\n Test coverage start");
        }

        private void EventsOnSessionEnd(object sender, SessionEndEventArgs e)
        {
            File.AppendAllText("debug.txt", $"\n\n Test coverage end {e.Context.TestCase.Source}");

            try
            {
                // Read coverage that was registered by: `Faultify.Injection.CoverageRegistry.RegisterTestCoverage()`.
                var binary = File.ReadAllBytes(TestRunnerConstants.CoverageFileName);

                var mutationCoverage = MutationCoverage.Deserialize(binary);

                // Filter out functions that are not tests
                mutationCoverage.Coverage = mutationCoverage.Coverage
                    .Where(pair => _testNames.Contains(pair.Key))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                var outputJson = mutationCoverage.Serialize();
                File.WriteAllBytes(TestRunnerConstants.CoverageFileName, outputJson);
            }
            catch (Exception ex)
            {
                File.AppendAllText("debug.txt", $"\n\ncoverage exception {ex}");
                // ignored
            }
        }

        private void EventsOnTestCaseEnd(object sender, TestCaseEndEventArgs e)
        {
            File.AppendAllText("debug.txt", $"\n\n Test case end");
            _testNames.Add(e.TestElement.FullyQualifiedName);
        }
    }
}