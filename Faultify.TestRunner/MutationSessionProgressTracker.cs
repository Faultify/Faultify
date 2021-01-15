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
            Log("Starting Building test project...");
        }

        public void LogEndPreBuilding()
        {
            _currentPercentage = 15;
            Log("Finished Building test project...");
        }

        public void LogBeginCoverage()
        {
            Log("Starting Coverage...");
        }

        public void LogEndCoverage()
        {
            _currentPercentage = 20;
            Log("Finished Coverage...");
        }

        public void LogBeginCleanBuilding()
        {
            Log("Starting Clean Building test project...");
        }

        public void LogEndCleanBuilding()
        {
            _currentPercentage = 35;
            Log("Finished Clean Building test project...");
        }

        public void LogBeginTestSession(int totalTestRounds)
        {
            Log($"Start Mutation Test Session. This takes {totalTestRounds} test rounds");
        }

        public void LogEndTestSession(TimeSpan elapsed)
        {
            Log($"Finished Mutation Test Session in {elapsed} time.");
        }

        public void LogBeginTestRun(int index, int max)
        {
            Log($"Starting Mutation Run [{DateTime.Now.TimeOfDay}]: {index}/{max}...");
        }

        public void LogEndTestRun(int index, int max, TimeSpan elapsedSinceStart)
        {
            _currentPercentage = (int) Map(index, 0f, max, 35f, 95f);
            Log($"Finished Mutation Run [{DateTime.Now.TimeOfDay}] elapsed: [{elapsedSinceStart}]: {index}/{max}...");
        }

        public void LogBeginGeneratingReport()
        {
            Log("Starting generating Report...");
        }

        public void LogEndGeneratingReport()
        {
            _currentPercentage = 100;
            Log("Finnish generating Report...");
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