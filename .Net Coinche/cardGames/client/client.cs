using System;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.TCP;
using Protocol;
using System.Threading;

namespace Client
{
    class Client
    {
        Network net;
        Display print;

        public void Connect()
        {
            net = new Network();
            print = new Display();
            ConnectionInfo connInfo = new ConnectionInfo("127.0.0.1", 2222);
            Connection newTCPConn = TCPConnection.GetConnection(connInfo);
            newTCPConn.AppendIncomingPacketHandler<Network>("Network", PrintIncomingMessage);
            newTCPConn.AppendShutdownHandler(OnConnectionClosed);
            Thread.Sleep(100);
            while (true) ;
        }
        private void OnConnectionClosed(Connection connection)
        {
            Console.WriteLine("Connection closed!");
            Environment.Exit(0);
        }

        private void OnConnectionEstablished(Connection connection)
        {
            Console.WriteLine("Connection established!");
        }

        private void PrintIncomingMessage(PacketHeader header, Connection connection, Network receive)
        {
            print.setNet(receive);
            if (receive.State == 0)
                print.display();
            //print msg
            else if (receive.State == 1)
                Console.WriteLine("         " + receive.message + Tools.ENTER);
            //print game + msg + get input from user
            else if (receive.State >= 2)
            {
                net = receive;
                if (receive.State == 2)
                    print.display();
                if (receive.State == 3)
                {
                    print.display_contract();
                    Console.WriteLine(Tools.ENTER);
                }
                print.check_and_send(connection);
            }
        }
    }
}