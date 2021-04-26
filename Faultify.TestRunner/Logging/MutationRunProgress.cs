namespace Faultify.TestRunner.Logging
{
    public struct MutationRunProgress
    {
        public MutationRunProgress(string message, int progress, LogMessageType logMessageType)
        {
            Progress = progress;
            LogMessageType = logMessageType;
            Message = message;
        }

        public string Message { get; set; }
        public int Progress { get; set; }
        public LogMessageType LogMessageType { get; }
    }
}
