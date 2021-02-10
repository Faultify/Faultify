using System;
using Microsoft.Extensions.Logging;

namespace Faultify.TestRunner.Logging
{
    /// <summary>
    ///     Helper class for tracking the mutation test logs and percentual progress.
    /// </summary>
    public class MutationSessionProgressTracker : IProgress<string>
    {
        private readonly ILogger _logger;
        private readonly IProgress<MutationRunProgress> _progress;

        private int _currentPercentage;

        public MutationSessionProgressTracker(IProgress<MutationRunProgress> progress, ILoggerFactory logger)
        {
            _progress = progress;
            _logger = logger.CreateLogger("Faultify.TestRunner");
        }

        public void Report(string value)
        {
            Log(value);
        }


        public void LogBeginPreBuilding()
        {
            _currentPercentage = 0;
            Log("Starting Building test project...");
        }

        public void LogEndPreBuilding()
        {
            _currentPercentage = 5;
            Log("Finished Building test project...");
        }

        public void LogBeginProjectDuplication(int duplications)
        {
            _currentPercentage = 7;
            Log($"Duplicating {duplications} test projects");
        }

        public void LogEndProjectDuplication()
        {
            _currentPercentage = 15;
            Log("End duplication test project...");
        }

        public void LogBeginCoverage()
        {
            _currentPercentage = 17;
            Log("Calculating Covered Mutations:\n" +
                "| - Inject code coverage functions.\n" +
                "| - Run test session\n" +
                "| - Calculate optimal way to execute most mutations in the least amount of test runs.",
                LogMessageType.CodeCoverage);
        }

        public void LogBeginTestSession(int totalTestRounds, int mutationCount, TimeSpan testRunTime)
        {
            _currentPercentage = 20;

            Log("Start Mutation Test Session:\n" +
                $"| Test Rounds: {totalTestRounds}\n" +
                $"| Mutations Found: {mutationCount}\n" +
                $"| Worst Case Time: {totalTestRounds * testRunTime.Seconds}s"
                , LogMessageType.TestSessionStart
            );
        }

        public void LogTestRunUpdate(int index, int max, int failedRuns)
        {
            _currentPercentage = (int) Map(index, 0f, max, 0f, 100f);
            Log("Test Run Progress:\n" +
                $"| Test Runs: {max - index}\n" +
                $"| Completed: {index}\n" +
                $"| Failed: {failedRuns}" +
                "", LogMessageType.TestRunUpdate);
        }

        public void LogEndTestSession(TimeSpan elapsed, int completedTestRounds, int mutationCount, float score)
        {
            _currentPercentage = 85;

            double mutationPerSeconds = (float)elapsed.Seconds == 0.0 ? 0.0 : mutationCount / (float)elapsed.Seconds;
 
             Log("Finished Mutation Session:\n" +
                $"| Test Rounds: {completedTestRounds}\n" +
                $"| Mutation per Second: {mutationPerSeconds:0.0}mps\n" +
                $"| Duration: {elapsed:hh\\:mm\\:ss}\n" +
                $"| Score: {score:0.0}%" +
                "\n", LogMessageType.TestSessionEnd
            );
        }

        public void LogBeginReportBuilding(string reportType, string reportPath)
        {
            _currentPercentage = 98;
            Log("Generate Report:\n" +
                $"| Report Path: {reportPath} \t\t \n" +
                $"| Report Type: {reportType} \t\t \n"
            );
        }

        public void LogEndFaultify(string processLog)
        {
            _currentPercentage = 100;
            Log("Faultify is Done:\n" +
                $"| Logs: {processLog} \t\t"
            );
        }

        public void LogDebug(string message)
        {
            _logger.LogDebug(message);
        }

        public void Log(string message, LogMessageType logMessageType = LogMessageType.Other)
        {
            _logger.LogInformation($"> [{_currentPercentage}] {message}");
            _progress.Report(new MutationRunProgress(message, _currentPercentage, logMessageType));
        }

        private static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}