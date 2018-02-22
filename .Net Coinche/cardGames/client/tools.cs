using System;
using System.Collections.Generic;

namespace Client
{
    class Tools
    {
        public static readonly String ACE = "Ace";
        public static readonly String KING = "King";
        public static readonly String QUEEN = "Queen";
        public static readonly String JACK = "Jack";
        public static readonly String HEARTS = " of hearts ";
        public static readonly String SPADES = " of spades ";
        public static readonly String DIAMONDS = " of diamonds ";
        public static readonly String CLUBS = " of clubs ";
        public static readonly String SCOREY = "Your Score : ";
        public static readonly String SCOREO = "Opponent Score : ";
        public static readonly String TABLE = "Table :\n";
        public static readonly String DECK = "\n\nDeck :\n";
        public static readonly String EMPTY = "No cards yet\n";
        public static readonly String SPLIT = "*************\n\n";
        public static readonly String PLAYER = "PLAYER ";
        public static readonly String ENTER = "\n";

        public static bool CheckTable(List<int> table)
        {
            for (int i = 0; i < 4; i++)
            {
                if (table[i] != -1)
                    return (true);
            }
            return (false);
        }
    }
}
