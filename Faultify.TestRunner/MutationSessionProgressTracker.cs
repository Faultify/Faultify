using System;

namespace Faultify.TestRunner
{
    /// <summary>
    ///     Helper class for tracking the mutation test logs and percentual progress.
    /// </summary>
    public class MutationSessionProgressTracker : IProgress<string>
    {
        private readonly IProgress<MutationRunProgress> _progress;

        private int _currentPercentage;

        public MutationSessionProgressTracker(IProgress<MutationRunProgress> progress)
        {
            _progress = progress;
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
            _currentPercentage = 10;
            Log("Finished Building test project...");
        }

        public void LogBeginProjectDuplication()
        {
            _currentPercentage = 15;
            Log("Starting duplicating test projects...");
        }

        public void LogEndProjectDuplication()
        {
            _currentPercentage = 20;
            Log("End duplication test project...");
        }


        public void LogBeginCoverage()
        {
            _currentPercentage = 22;
            Log("Starting Coverage...");
        }

        public void LogEndCoverage()
        {
            _currentPercentage = 25;
            Log("Finished Coverage...");
        }

        public void LogBeginTestSession(int totalTestRounds)
        {
            Log($"Start Mutation Test Session. This takes {totalTestRounds} test rounds");
        }

        public void LogBeginTestRun(int runId)
        {
            Log($"Starting Mutation Run: '{runId}' [{DateTime.Now.TimeOfDay.ToString("hh\\:mm\\:ss")}]:...");
        }

        public void LogEndTestRun(int index, int max, int runId, TimeSpan elapsedSinceStart)
        {
            _currentPercentage = (int) Map(index, 0f, max, 35f, 95f);
            Log($"Finished Mutation Run '{runId}', elapsed: [{elapsedSinceStart.ToString("hh\\:mm\\:ss")}]: ...");
        }

        public void LogEndTestSession(TimeSpan elapsed)
        {
            _currentPercentage = 100;
            Log($"Finished Mutation Test Session in {elapsed.ToString("hh\\:mm\\:ss")} time.");
        }

        public void Log(string message)
        {
            _progress.Report(new MutationRunProgress(message, _currentPercentage));
        }

        private static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}