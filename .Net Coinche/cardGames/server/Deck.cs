using System;
using System.Collections.Generic;

namespace Server
{
    class Deck
    {
        public List<Int32> deck;
        public int[] noAtout = { 19, 0, 0, 0, 10, 2, 3, 4 };
        public int[] atout = { 6, 0, 0, 9, 5, 14, 1, 3 };

        public Deck()
        {
            deck = new List<Int32>();
        }

        public void initDeck()
        {
            int i = 1;
            while (i != 33)
            {
                deck.Add(i);
                i++;
            }
        }
        public void RandomOrder()
        {
            Random r = new Random();
            for (int cnt = 0; cnt < deck.Count; cnt++)
            {
                int tmp = deck[cnt];
                int idx = r.Next(deck.Count - cnt) + cnt;
                deck[cnt] = deck[idx];
                deck[idx] = tmp;
            }
        }

        public List<Int32> getDeck()
        {
            return deck;
        }


        public int[] getNoAtout()
        {
            return noAtout;
        }

        public int[] getAtout()
        {
            return atout;
        }
    }
}
