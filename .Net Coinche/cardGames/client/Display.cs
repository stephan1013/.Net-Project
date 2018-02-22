using System;
using System.Collections.Generic;
using Protocol;
using NetworkCommsDotNet.Connections;
using System.Threading;

namespace Client
{
    class Display
    {
        Network net;

        public Display()
        {
            net = new Network();
        }

        public void setNet(Network Net)
        {
            net = Net;
        }

        public void print_card(int nb, int card_nb)
        {
            if (nb != -1)
            {
                String card;
                int id;

                id = nb % 8;
                // nb of the card
                if (id == 1)
                    card = Tools.ACE;
                else if (id == 0)
                    card = Tools.KING;
                else if (id == 7)
                    card = Tools.QUEEN;
                else if (id == 6)
                    card = Tools.JACK;
                else
                    card = (id + 5).ToString();
                // color of the card
                if (nb >= 1 && nb <= 8)
                    card += Tools.HEARTS;
                else if (nb >= 9 && nb <= 16)
                    card += Tools.SPADES;
                else if (nb >= 17 && nb <= 24)
                    card += Tools.DIAMONDS;
                else if (nb >= 25 && nb <= 32)
                    card += Tools.CLUBS;
                Console.Write("[" + card_nb + "]" + card + " ");
                if (card_nb == 4)
                    Console.WriteLine(Tools.ENTER);
            }
        }

        public void display()
        {
            //print score
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Tools.SPLIT);
            Console.WriteLine(Tools.SCOREY + ((net.Id % 2 == 0) ? net.ScoreA : net.ScoreB) + Tools.ENTER);
            Console.WriteLine(Tools.SCOREO + ((net.Id % 2 == 1) ? net.ScoreA : net.ScoreB) + Tools.ENTER);

            // print contract
            if (net.bet.Count != 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                display_contract();
                Console.WriteLine(Tools.ENTER);
            }

            // print table
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(Tools.TABLE);
            if (Tools.CheckTable(net.table) == false)
                Console.WriteLine(Tools.EMPTY);
            else
            {
                int id = net.Id;
                for (int count = 0; count < net.table.Count; count++)
                {
                    if (id == 0)
                        id = 4;
                    id--;
                }
                for (int i = 0; i < net.table.Count; i++)
                {
                    print_card(net.table[i], id);
                    if (id == 3)
                        id = -1;
                    id++;
                }

            }
            //print deck
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Tools.DECK);
            if (net.Hand.Count == 0)
                Console.WriteLine(Tools.EMPTY);
            else
                for (int y = 0; y < net.Hand.Count; y++)
                    print_card(net.Hand[y], y + 1);
            Console.WriteLine(Tools.ENTER + Tools.ENTER + Tools.SPLIT + Tools.ENTER);

        }

        public void display_contract()
        {
            for (int i = 0; i < net.bet.Count; ++i)
            {
                if (net.bet[i] != -1)
                {
                    Console.Write(Tools.PLAYER + i + " bet " + net.bet[i] + " points");
                    if (net.color[i] == 1)
                        Console.WriteLine(Tools.HEARTS + Tools.ENTER);
                    else if (net.color[i] == 2)
                        Console.WriteLine(Tools.SPADES + Tools.ENTER);
                    else if (net.color[i] == 3)
                        Console.WriteLine(Tools.DIAMONDS + Tools.ENTER);
                    else if (net.color[i] == 4)
                        Console.WriteLine(Tools.CLUBS + Tools.ENTER);
                }
            }
        }

        public int max(List<Int32> list)
        {
            int maxAge = 0;
            foreach (int type in list)
            {
                if (type > maxAge)
                    maxAge = type;
            }
            return maxAge;
        }

        public void check_and_send(Connection connection)
        {
            String send;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("         " + net.message + Tools.ENTER);
                send = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                try
                {
                    if (net.State == 2)
                    {
                        if (send.Length == 1 && Int32.Parse(send) <= net.Hand.Count && Int32.Parse(send) > 0
                            && net.Hand[Int32.Parse(send) - 1] != -1)
                            break;
                        else
                            Console.WriteLine("The card is out of range\n\n");
                    }
                    else if (net.State == 3)
                    {
                        if (send.Length == 1 && (Int32.Parse(send) == 1 || Int32.Parse(send) == 2))
                            break;
                        else
                            Console.WriteLine("Write 1 or 2 to take the contract or not\n\n");

                    }
                    else if (net.State == 4)
                    {
                        if (send.Length == 1 && Int32.Parse(send) > 0 && Int32.Parse(send) < 5)
                            break;
                        else
                            Console.WriteLine("Write the number of the color between 1 and 4\n\n");
                    }
                    else if (net.State == 5)
                    {
                        if (send.Length < 4 && (net.bet.Count == 0 || Int32.Parse(send) > max(net.bet))
                                && Int32.Parse(send) <= 160 && Int32.Parse(send) % 10 == 0)
                            break;
                        else
                            Console.WriteLine("Write a bet between 80 and 160\n\n");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Write an Interger\n\n");
                }
            }
            net.message = send;
            connection.SendObject("Network", net);
            Thread.Sleep(net.SLEEP);
            Console.WriteLine(Tools.ENTER);
        }
    }
}
