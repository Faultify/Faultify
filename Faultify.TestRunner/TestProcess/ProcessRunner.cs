using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Faultify.TestRunner.TestProcess
{
    /// <summary>
    ///     Async process runner.
    /// </summary>
    public class ProcessRunner
    {
        private readonly ProcessStartInfo _processStartInfo;

        /// <summary>
        /// The output message of this process.
        /// </summary>
        public StringBuilder Output;
        
        /// <summary>
        /// The error message of this process. 
        /// </summary>
        public StringBuilder Error;

        public ProcessRunner(ProcessStartInfo processStartInfo)
        {
            _processStartInfo = processStartInfo;
        }

        /// <summary>
        /// Runs the process asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<Process> RunAsync()
        {
            var process = new Process();
            
            var taskCompletionSource = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (o, e) =>
            {
                taskCompletionSource.TrySetResult(null);
            };

            process.StartInfo = _processStartInfo;

            Output = new StringBuilder();
            Error = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                Output.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                Error.AppendLine(e.Data);
            };
            
            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await taskCompletionSource.Task;

            process.WaitForExit();

            return process;
        }
    }
}