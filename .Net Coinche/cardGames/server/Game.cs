using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Protocol;


namespace Server
{


    class Game
    {
        public bool init;
        public int scoreA;
        public int scoreB;
        public int play;
        public int turn;
        public int round;
        public List<int> lcolor;
        public List<int> lbet;
        Network net;
        List<Client> players;
        List<Int32> table;
        int bet;
        int color;
        Deck deck;

        public Game()
        {
            net = new Network();
            deck = new Deck();
            lbet = new List<int>();
            lcolor = new List<int>();
            net.Hand = new List<int>();
            table = new List<int>();
            init = false;
        }

        public void DistributCard()
        {
            int index = 0;

            deck.RandomOrder();
            for (int i = 0; i < 4; i++)
            {
                players[i].hand.Clear();
                (players[i]).play = true;
                for (int j = 0; j < 8; j++)
                {
                    (players[i]).hand.Add(deck.deck[index]);
                    ++index;
                }
            }
        }

        public void InitGame(List<Client> Players)
        {
            players = Players;
            if (init == false)
            {
                for (int i = 0; i < 4; i++)
                    lbet.Add(-1);
                for (int i = 0; i < 4; i++)
                    table.Add(-1);
                for (int i = 0; i < 4; i++)
                    lcolor.Add(-1);
                deck.initDeck();
                init = true;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                    lbet[i] = -1;
                for (int i = 0; i < 4; i++)
                    table[i] = -1;
                for (int i = 0; i < 4; i++)
                    lcolor[i] = -1;
                init = false;
            }
            play = 0;
            turn = 0;
            round = 0;
            scoreA = 0;
            scoreB = 0;
            round = 0;
            bet = 0;
            color = 0;
            DistributCard();
            StartGame();
        }

        public void SendMessage(string msg, int state, Client client)
        {
            net.message = msg;
            net.Id = client.id;
            net.ScoreA = scoreA;
            net.ScoreB = scoreB;
            net.State = state;
            net.Hand = client.hand;
            net.table = table;
            net.bet = lbet;
            net.color = lcolor;
            client.connect.SendObject("Network", net);
            Thread.Sleep(net.SLEEP);
        }

        bool CheckTable(List<int> table)
        {
            for (int i = 0; i < 8; i++)
            {
                if (players[0].hand[i] != -1)
                    return (true);
            }
            return (false);
        }

        private void CalculateScore()
        {
            int win = 0;
            int card = 0;
            int tmp;
            Boolean atout;
            Boolean cut = false;
            int sum = 0;

            for (int i = 0; i < players.Count(); i++)
            {
                if (color == ((players[i].lcard - 1) / 8))
                {
                    atout = true;
                    tmp = deck.getAtout()[((players[i].lcard - 1) % 8)];
                }
                else
                {
                    atout = false;
                    tmp = deck.getNoAtout()[((players[i].lcard - 1) % 8)];
                }
                if (!cut && (card < tmp) && !atout)
                {
                    win = i;
                    card = tmp;
                }
                else if (atout)
                {
                    if (!cut)
                    {
                        cut = true;
                        win = i;
                        card = tmp;
                    }
                    else
                    {
                        if (card < tmp)
                        {
                            win = i;
                            card = tmp;
                        }
                    }
                }
                sum += tmp;
            }
            if ((players[win].id % 2) == 0)
            {
                scoreA += sum;
                for (int i = 0; i < players.Count; i++)
                {
                    SendMessage("Team A win " + sum + " point !", 1, players[i]);
                }
            }
            else
            {
                scoreB += sum;
                for (int i = 0; i < players.Count; i++)
                {
                    SendMessage("Team B win " + sum + " point !", 1, players[i]);
                }
            }
            for (int i = 0; i < 4; i++)
                net.table[i] = -1;
            if (CheckTable(table) == true)
            {
                play = win;
                ++round;
                SendMessage(net.PICK, 2, players[play]);
            }
            else
            {
                if ((scoreA + 10 + bet) >= 500 || (scoreB + 10 + bet) >= 500)
                {

                    String winner = (scoreA >= 510 + bet) ? "Team A win the Game with " : "Team B win the game with ";
                    int score = (scoreA >= 510 + bet) ? scoreA + 10 + bet : scoreB + 10 + bet;
                    for (int i = 0; i < players.Count; i++)
                    {
                        SendMessage(winner + score + " point", 1, players[i]);
                    }
                    InitGame(players);
                    StartGame();
                }
                else
                {
                    String winner = "";
                    if (scoreA > scoreB)
                    {
                        scoreA += bet + 10;
                        winner = "Team A win this round and have " + scoreA + " point";
                    }
                    else
                    {
                        scoreB += bet + 10;
                        winner = "Team B win this round and have " + scoreB + " point";
                    }
                    for (int i = 0; i < 4; i++)
                        SendMessage(winner, 1, players[i]);
                    StartGame();
                }
            }
        }

        private void ChooseCard(Network net)
        {
            table[play] = (net.Hand[(Int32.Parse(net.message) - 1)]);
            players[play].lcard = net.Hand[(Int32.Parse(net.message) - 1)];
            players[play].hand[(Int32.Parse(net.message) - 1)] = -1;
            for (int i = 0; i < players.Count; i++)
                SendMessage("Game\n", 0, players[i]);
            Thread.Sleep(net.SLEEP);
            turn += 1;
            play = (play + 1) == 4 ? 0 : (play + 1);
            if (turn < 4)
                SendMessage(net.PICK, 2, players[play]);
            else
            {
                turn = 0;
                CalculateScore();
            }
        }

        public void ChooseBet(Network net)
        {
            int tmp = Int32.Parse(net.message);
            lbet[play] = tmp;
            if (bet < tmp)
            {
                bet = tmp;
                color = lcolor[play];
            }
            turn += 1;
            if (bet != 160)
                play = (play + 1) == 4 ? 0 : (play + 1);
            if (turn < 4 && bet != 160)
                SendMessage(net.CONTRACT, 3, players[play]);
            else
            {
                turn = 0;
                for (int i = 0; i < lbet.Count; i++)
                    if (bet != lbet[i])
                    {
                        lbet[i] = -1;
                        lcolor[i] = -1;
                    }
                color -= 1;
                SendMessage(net.PICK, 2, players[play]);
            }
        }

        private void ChooseColor(Network net)
        {
            int tmp = Int32.Parse(net.message);
            lcolor[play] = tmp;
            SendMessage(net.BET, 5, players[play]);
        }

        private void ChooseContrat(Network net)
        {
            if (Int32.Parse(net.message) == 1)
            {
                SendMessage(net.COLOR, 4, players[play]);
            }
            else
            {
                turn += 1;
                play = (play + 1) == 4 ? 0 : (play + 1);
                if (turn < 4)
                    SendMessage(net.CONTRACT, 3, players[play]);
                else
                {
                    turn = 0;
                    for (int i = 0; i < lbet.Count; i++)
                    {
                        if (bet != lbet[i])
                        {
                            lbet[i] = -1;
                            lcolor[i] = -1;
                        }
                    }
                    if (bet == 0)
                    {
                        for (int i = 0; i < players.Count; i++)
                            players[i].hand.Clear();
                        StartGame();
                    }
                    else
                        SendMessage(net.PICK, 2, players[play]);
                }
            }
        }

        public void NextAction(Network net)
        {
            if (net.State == 2)
                ChooseCard(net);
            else if (net.State == 3)
                ChooseContrat(net);
            else if (net.State == 4)
                ChooseColor(net);
            else if (net.State == 5)
                ChooseBet(net);
        }

        public void StartGame()
        {
            round = 0;
            DistributCard();
            for (int i = 0; i < players.Count; i++)
                SendMessage("Start Round ", 0, players[i]);
            Thread.Sleep(net.SLEEP);
            SendMessage("Contract : Take(1), Pass(2)", 3, (Client)players[play]);
        }
    }
}
