using System.IO.Pipes;
using System.Reflection;

namespace Shared
{
    internal class Pipe
    {
        private static readonly string PipeName = "NamedPipe-csharp-study_OneWayConsole_Pipe";

        public static async Task SendAsync(string? msg, TimeSpan timeout = default)
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

        public static async Task Server(Action<string?> action, CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    using var pipe = new NamedPipeServerStream(PipeName);
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
