using System;
using System.Collections.Generic;
using NetworkCommsDotNet.Connections;

namespace Server
{
    class Client
    {
        public int id;
        public int lcard;
        public List<Int32> hand;
        public Connection connect;
        public Boolean play;

        public Client(int Id, Connection Connect, Boolean Play)
        {
            id = Id;
            connect = Connect;
            play = Play;
            lcard = 0;
            hand = new List<Int32>();
        }
    }
}
