using System;
using System.Collections.Generic;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using Protocol;
using System.Threading;

namespace Server
{
    class Server
    {
        public Network net;
        public List<Client> waiting;
        public int id;
        public bool play;
        public Game game;
        public List<Client> tmp;


        public Server()
        {
            net = new Network();
            game = new Game();
            waiting = new List<Client>();
            tmp = new List<Client>();
            play = false;
            id = 0;
        }

        public void Connect()
        {
            //Trigger the method PrintIncomingMessage when a packet of type 'Network' is received
            NetworkComms.AppendGlobalIncomingPacketHandler<Network>("Network", PrintIncomingMessage);
            NetworkComms.AppendGlobalConnectionEstablishHandler(OnConnectionEstablished);
            NetworkComms.AppendGlobalConnectionCloseHandler(OnConnectionClosed);

            Thread.Sleep(net.SLEEP);
            //Start listening for incoming connections
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 2222));

            //Print out the IPs and ports we are now listening on
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

            Console.WriteLine("\ntype quit' to close server.");
            while (Console.ReadLine() != "quit") ;
            //We have used NetworkComms so we should ensure that we correctly call shutdown
            NetworkComms.Shutdown();
        }

        private void OnConnectionClosed(Connection connection)
        {
            Console.WriteLine("Connection closed!");
            if (play == true)
            {
                play = false;
                for (int i = 0; i < tmp.Count; i++)
                {
                    ((Client)tmp[i]).connect.CloseConnection(false);
                    id -= 1;
                }
                for (int i = 0; i < 4; i++)
                    waiting.RemoveAt(0);
                tmp.Clear();
            }
        }

        private void OnConnectionEstablished(Connection connection)
        {
            Console.WriteLine("Connection established!");
            Client newclient = new Client(id, connection, false);
            game.SendMessage("Welcome to Coinche Game", 1, newclient);
            waiting.Add(newclient);
            if (id == 3)
            {
                play = true;
                Console.WriteLine("Game Start");
                for (int i = 0; i < 4; i++)
                    tmp.Add(waiting[i]);
                game.InitGame(tmp);
            }
            ++id;
            Thread.Sleep(net.SLEEP);
        }

        private void PrintIncomingMessage(PacketHeader header, Connection connect, Network message)
        {
            if (id >= 3)
                game.NextAction(message);
        }
    }
}