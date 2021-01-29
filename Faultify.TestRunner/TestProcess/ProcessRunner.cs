using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Faultify.TestRunner.TestProcess
{
    /// <summary>
    ///     Async process runner.
    /// </summary>
    public class ProcessRunner
    {
        private readonly ProcessStartInfo _processStartInfo;

        public ProcessRunner(ProcessStartInfo processStartInfo)
        {
            _processStartInfo = processStartInfo;
        }

        public async Task<Process> RunAsync(CancellationToken cancellationToken)
        {
            var process = new Process();

            var cancellationTokenRegistration = cancellationToken.Register(() => { process.Kill(true); });

            var taskCompletionSource = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (o, e) => { taskCompletionSource.TrySetResult(null); };
            process.StartInfo = _processStartInfo;
            
            process.Start();
            
            await taskCompletionSource.Task;
            await cancellationTokenRegistration.DisposeAsync();
            
            return process;
        }
    }
}