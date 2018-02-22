using System;
using System.Collections.Generic;
using ProtoBuf;


namespace Protocol
{
    [ProtoContract]
    public class Network
    {
        public Network()
        {
        }

        [ProtoMember(1)]
        public string message { get; set; }

        [ProtoMember(2)]
        public int Id { get; set; }

        [ProtoMember(3)]
        public int ScoreA { get; set; }

        [ProtoMember(4)]
        public int ScoreB { get; set; }

        [ProtoMember(5)]
        public int State { get; set; }

        [ProtoMember(6, DynamicType = true)]
        public List<Int32> Hand { get; set; }

        [ProtoMember(7)]
        public List<Int32> table { get; set; }

        [ProtoMember(8)]
        public List<Int32> bet { get; set; }

        [ProtoMember(9)]
        public List<Int32> color { get; set; }

        [ProtoMember(10)]
        public readonly String CONTRACT = "Contract : Take(1), Pass(2)";

        [ProtoMember(11)]
        public readonly String COLOR = "Choose color : HEARTS(1), SPADES(2), DIAMONDS(3), CLUBS(4)";

        [ProtoMember(12)]
        public readonly String BET = "Bet :(80,90,100,110,120,130,140,150,160) You must bet more than the last bet";

        [ProtoMember(13)]
        public readonly String PICK = "Put a card : ";

        [ProtoMember(14)]
        public readonly int SLEEP = 200;
    }
}
