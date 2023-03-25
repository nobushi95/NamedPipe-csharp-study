using Shared;

namespace ClientConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
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