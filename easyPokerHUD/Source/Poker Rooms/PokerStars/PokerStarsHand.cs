using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace easyPokerHUD
{
    public class PokerStarsHand : PokerRoomHand
    {
        public PokerStarsHand(string path)
        {
            
            this.path = path;
            pokerRoom = "PokerStars";

            try
            {
                //Read in the hand from the txt file
                hand = getHand(path, "SUMMARY", "");

                //Store the general information about the hand in separate strings
                handInformation = hand[0];
                tableInformation = hand[1];

                //Separate the hand into pieces
                playerOverview = hand.SkipWhile(s => !s.Contains("chips")).TakeWhile(s => s.Contains("Seat ")).ToArray();
                preflop = hand.SkipWhile(s => !s.Contains("HOLE CARDS")).TakeWhile(s => !s.Contains("FLOP")).ToArray();
                postflop = hand.SkipWhile(s => !s.Contains("FLOP")).TakeWhile(s => !s.Contains("SHOW DOWN") || !s.Contains("SUMMARY")).ToArray();
                bigBlind = getBigBlind(hand.SkipWhile(s => !s.Contains("posts")).TakeWhile(s => !s.Contains("HOLE CARDS")).ToArray().Last());

                //Get the table name and table size from the tableinformation string
                tableName = getTableName(tableInformation);
                tableSize = getTableSize(tableInformation);

                //Get the players with stats playing in this hand
                players = getPlayersWithStats(playerOverview, bigBlind, preflop, postflop, pokerRoom);

                //Get the player name of this hand
                playerName = getPlayerName(hand);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (NotImplementedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Gets the table name of the hand
        /// </summary>
        /// <param name="tableInformation"></param>
        /// <returns></returns>
        protected string GetTableName(string tableInformation)
        {
            string tableName = tableInformation.Substring(tableInformation.IndexOf("'") + 1);
            tableName = tableName.Substring(0, tableName.IndexOf("'"));
            try
            {
                if (tableName.Substring(0, tableName.IndexOf(" ") - 1).All(char.IsDigit))
                {
                    tableName = tableName.Substring(0, tableName.IndexOf(" ") - 1);
                }
            }
            catch
            {
            }
            return tableName;
        }

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="tableInformation"></param>
        /// <returns></returns>
        protected int GetTableSize(string tableInformation)
        {
            string stringThatContainsTableSize = tableInformation.Substring(tableInformation.IndexOf("'") + 1);
            stringThatContainsTableSize = stringThatContainsTableSize.Substring(stringThatContainsTableSize.IndexOf("'") + 1);
            stringThatContainsTableSize = Regex.Match(stringThatContainsTableSize, @"\d+").Value;
            int tableSize = int.Parse(stringThatContainsTableSize);
            return tableSize;
        }

        /// <summary>
        /// Gets the name of this hands player
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        protected string GetPlayerName(string[] hand)
        {
            foreach (string line in hand)
            {
                if (line.Contains("Dealt"))
                {
                    foreach (Player player in players)
                    {
                        if (line.Contains(player.name))
                        {
                            return player.name;
                        }
                    }
                }
            }
            return "0";
        }

        //Creates a list of players
        public static List<Player> getPlayersWithStats(string[] playerOverview, int bigBlind, string[] preflop, string[] postflop,
             string pokerRoom)
        {
            //Create a list for all the players
            List<Player> players = new List<Player>();

            foreach (string line in playerOverview)
            {
                Player player = new Player(GetName(line))
                {
                    seat = GetSeatNumber(line),
                    pokerRoom = pokerRoom,
                    handsPlayed = 1
                };
                players.Add(player);
            }

            players = insertChipStats(playerOverview, bigBlind, players, " in chips");
            players = insertPreFlopStats(preflop, players, "calls", "raises", "bets");
            players = insertPostFlopStats(postflop, players, "calls", "raises", "bets", "checks", "folds");
            return players;
        }

        /// <summary>
        /// Extracts the name out of a given line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        protected static string GetName(string line)
        {
            string name = line.Substring(line.IndexOf(":") + 2);
            name = name.Substring(0, name.IndexOf("(") - 1);
            return name;
        }

        /// <summary>
        /// Extracts the seatnumber out of a given line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        protected static int GetSeatNumber(string line)
        {
            string resultString = Regex.Match(line, @"\d+").Value;
            int seatNumber = int.Parse(resultString);
            return seatNumber;
        }

        //Extracts the big blinds out of a given line
        protected static int getBigBlind(String line)
        {
            return Int32.Parse(Regex.Match(line, @"\d+$").Value);
        }
    }
}
