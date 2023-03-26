using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    internal class Pipe
    {
        private static readonly string PipeName = "NamedPipe-csharp-study_OneWayWPF_Pipe";

        public async Task SendAsync(string? msg, TimeSpan timeout = default)
        {
            using var pipe = new NamedPipeClientStream(PipeName);
            try
            {
                if (timeout == default)
                    await pipe.ConnectAsync();
                else
                    await pipe.ConnectAsync((int)timeout.TotalMilliseconds);

                using var sw = new StreamWriter(pipe);
                await sw.WriteLineAsync(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Catch Exception at {MethodBase.GetCurrentMethod()?.Name}(): {ex.Message}");
            }
        }

        public async Task Server(Action<string?> action, CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    // 複数のサーバーを立ち上げ、メッセージを送信すると、
                    // プロセスを起動した順にメッセージが受信される？
                    using var pipe = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 3);
                    await pipe.WaitForConnectionAsync(cancel);

                    using var sr = new StreamReader(pipe);
                    var msg = await sr.ReadLineAsync();

                    action.Invoke(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Catch Exception at {MethodBase.GetCurrentMethod()?.Name}: {ex.Message}");
                    return;
                }
            }
        }
    }
}
