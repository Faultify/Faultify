namespace Faultify.TestRunner
{
    public struct MutationRunProgress
    {
        public MutationRunProgress(string message, int progress)
        {
            Progress = progress;
            Message = message;
        }

        public string Message { get; set; }
        public int Progress { get; set; }
    }
}