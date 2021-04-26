using System;

namespace Faultify.TestRunner.TestProcess
{
    public class ExitCodeException : Exception
    {
        public ExitCodeException(int exitCode) : base($"Process exited with exit code: {exitCode}")
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; }
    }
}
