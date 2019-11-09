using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace easyPokerHUD
{
    public class EightPokerHand : PokerRoomHand
    {
        public EightPokerHand(string path)
        {
            this.path = path;
            pokerRoom = "EightPoker";

            //Read in the hand from the txt file
            hand = GetHand(path, "Summary", "");

            //Store the general information about the hand in separate strings
            handInformation = hand[1];
            tableInformation = hand[3];

            //Separate the hand into pieces
            playerOverview = hand.TakeWhile(s => !s.Contains("Dealing down cards")).ToArray();
            playerOverview = hand.Reverse().SkipWhile(s => !s.Contains("Seat")).TakeWhile(s => s.Contains("Seat ")).Reverse().ToArray();
            preflop = hand.SkipWhile(s => !s.Contains("Dealing down cards")).TakeWhile(s => !s.Contains("Dealing flop")).ToArray();
            postflop = hand.SkipWhile(s => !s.Contains("Dealing flop")).TakeWhile(s => !s.Contains("Summary")).ToArray();

            //Get the table name and table size from the tableinformation string
            tableName = GetTableName(tableInformation);
            tableSize = GetTableSize(tableInformation);

            //Get the players with stats playing in this hand
            players = GetPlayersWithStats(playerOverview, preflop, postflop, pokerRoom);

            //Get the player name of this hand
            playerName = GetPlayerName(hand);
        }

        /// <summary>
        /// Gets the table name of the hand
        /// </summary>
        /// <param name="tableInformation"></param>
        /// <returns></returns>
        protected string GetTableName(string tableInformation)
        {
            string tableName = tableInformation.Substring(tableInformation.IndexOf(" ") + 1);
            tableName = tableName.Substring(0, tableName.IndexOf(" "));
            return tableName;
        }

        /// <summary>
        /// Gets the size of the table
        /// </summary>
        /// <param name="tableInformation"></param>
        /// <returns></returns>
        protected int GetTableSize(string tableInformation)
        {
            string stringThatContainsTableSize = tableInformation;
            if (tableInformation.Contains("Tournament"))
            {
                stringThatContainsTableSize = tableInformation.Substring(tableInformation.IndexOf("Max") - 3);
                stringThatContainsTableSize = Regex.Match(stringThatContainsTableSize, @"\d+").Value;
            }
            else
            {
                stringThatContainsTableSize = Regex.Match(tableInformation, @"\d+").Value;
            }
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
            return "";
        }

        /// <summary>
        /// Creates a list of players
        /// </summary>
        /// <param name="playerOverview"></param>
        /// <param name="preflop"></param>
        /// <param name="postflop"></param>
        /// <param name="pokerRoom"></param>
        /// <returns></returns>
        public static List<Player> GetPlayersWithStats(string[] playerOverview, string[] preflop, string[] postflop,
             string pokerRoom)
        {
            //Create a list for all the players
            List<Player> players = new List<Player>();

            //Go through the player overview and extract seat as well as name
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

            players = InsertPreFlopStats(preflop, players, "calls", "raises", "bets");
            players = InsertPostFlopStats(postflop, players, "calls", "raises", "bets", "checks", "folds");
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
    }
}
