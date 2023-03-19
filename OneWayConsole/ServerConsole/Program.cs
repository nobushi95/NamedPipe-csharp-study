using Shared;

namespace ServerConsole
{
    internal class Program
    {
        private static readonly CancellationTokenSource cancel = new();

        static async Task Main(string[] args)
        {
            Console.WriteLine("--- Server Console ---");
            await Pipe.Server(OnReceive, cancel.Token);
            Console.Write("Press enter key to exit.");
            Console.Read();
        }

        private static void OnReceive(string? msg)
        {
            if (msg == "exit")
            {
                cancel.Cancel();
            }
            Console.WriteLine($"Received message: \"{msg}\"");
        }
    }
}