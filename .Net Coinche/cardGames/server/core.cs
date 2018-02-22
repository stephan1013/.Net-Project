using System;
using System.Threading;

namespace Server
{
    class Core
    {
        static void Main(string[] args)
        {
            Server server;

            server = new Server();
            try
            {
                server.Connect();
            }
            catch (Exception)
            {
                Console.WriteLine("Server fail");
                Thread.Sleep(2000);
            }
        }
    }
}
