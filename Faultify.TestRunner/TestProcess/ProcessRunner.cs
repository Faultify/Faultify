using System.Diagnostics;
using System.Threading.Tasks;
using NLog;

namespace Faultify.TestRunner.TestProcess
{
    /// <summary>
    ///     Async process runner.
    /// </summary>
    public class ProcessRunner
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ProcessStartInfo _processStartInfo;

        public ProcessRunner(ProcessStartInfo processStartInfo)
        {
            _processStartInfo = processStartInfo;
        }

        /// <summary>
        ///     Runs the process asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<Process> RunAsync()
        {
            _logger.Info("Starting new process");
            Process? process = new Process();

            TaskCompletionSource<object>? taskCompletionSource = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (o, e) => { taskCompletionSource.TrySetResult(null); };

            process.StartInfo = _processStartInfo;

            process.OutputDataReceived += (sender, e) => { _logger.Debug(e.Data); };
            process.ErrorDataReceived += (sender, e) => { _logger.Error(e.Data); };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();


            await taskCompletionSource.Task;

            process.WaitForExit();

            return process;
        }
    }
}
