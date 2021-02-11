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

        private bool _coverageFlushed = false;
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

            AppDomain.CurrentDomain.ProcessExit += OnCurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.UnhandledException += OnCurrentDomain_ProcessExit;
        }

        private void EventsOnSessionStart(object sender, SessionStartEventArgs e)
        {
            _logger.LogWarning(context.SessionDataCollectionContext, "Coverage Test Session Started");
        }

        private void OnCurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _logger.LogWarning(context.SessionDataCollectionContext, "Coverage Test Session Exit");

            EventsOnSessionEnd(sender, new SessionEndEventArgs() {});
        }

        private void EventsOnSessionEnd(object sender, SessionEndEventArgs e)
        {
            try
            {
                if (_coverageFlushed) return;

                // Read coverage that was registered by: `Faultify.Injection.CoverageRegistry.RegisterTestCoverage()`.
                var binary = File.ReadAllBytes(TestRunnerConstants.CoverageFileName);

                var mutationCoverage = MutationCoverage.Deserialize(binary);

                // Filter out functions that are not tests
                mutationCoverage.Coverage = mutationCoverage.Coverage
                    .Where(pair => _testNames.Contains(pair.Key))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
                
                var serialized = mutationCoverage.Serialize();
                File.WriteAllBytes(TestRunnerConstants.CoverageFileName, serialized);
                _coverageFlushed = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(context.SessionDataCollectionContext, $"Test Session Exception: {ex}");
            }

            _logger.LogWarning(context.SessionDataCollectionContext, "Coverage Test Session Finished");
        }

        private void EventsOnTestCaseStart(object sender, TestCaseStartEventArgs e)
        {
            _logger.LogWarning(context.SessionDataCollectionContext, $"Test Case Start: {e.TestCaseName}");
        }

        private void EventsOnTestCaseEnd(object sender, TestCaseEndEventArgs e)
        {
            _logger.LogWarning(context.SessionDataCollectionContext, $"Test Case End: {e.TestCaseName}");
            _testNames.Add(e.TestElement.FullyQualifiedName);
        }
    }
}