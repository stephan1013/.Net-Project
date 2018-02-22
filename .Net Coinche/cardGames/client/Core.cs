using System;
using System.Threading;

namespace Client
{
    class Core
    {
        static void Main(string[] args)
        {
            Client client;

            client = new Client();
            try
            {
                client.Connect();
            }
            catch (Exception)
            {
                Console.SetCursorPosition(Console.LargestWindowHeight, 0);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Connection Fail");
                Thread.Sleep(2000);
            }

        }
    }
}
