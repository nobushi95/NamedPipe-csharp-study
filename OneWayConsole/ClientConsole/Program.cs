using Shared;

namespace ClientConsole
{
    internal class Program
    {
        private static readonly string MutexName = "OneWayConsole_ClientConsole_MutexName";
        private static readonly SingleInstance singleInstance = new(MutexName);
        static async Task Main(string[] args)
        {
            // 多重起動防止
            // 短絡評価で先にSingleInstanceで判定して、Mutexを作っておく
            // すでにプロセスが1つ以上存在すれば、-Singleオプションで呼ばれるプロセスは起動しない
            if (singleInstance.IsRunning() && args.Any(x => x == "-Single"))
                return;

            Console.WriteLine("--- Client Console ---");
            while (true)
            {
                Console.Write("> ");
                var msg = Console.ReadLine();
                await Pipe.SendAsync(msg, TimeSpan.FromSeconds(1));
                if (msg == "exit")
                    break;
            }
            Console.WriteLine("Press enter key to exit.");
            Console.Read();
        }
    }
}