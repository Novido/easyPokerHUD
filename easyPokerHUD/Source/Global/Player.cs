using System;

namespace easyPokerHUD
{
    public class Player
    {
        /// <summary>
        /// Variables of the Object "Player"
        /// </summary>
        public string name;
        public string pokerRoom;
        public bool hadActionInPot;
        public int seat;
        public int handsPlayed;
        public int chips;
        public double bigBlinds;
        public int preflopCalls;
        public int preflopBetsAndRaises;
        public int postflopBetsAndRaises;
        public int postflopCallsChecksAndFolds;

        public Player(string name)
        {
            this.name = name;
            this.pokerRoom = "";
            this.hadActionInPot = false;
            this.seat = 0;
            this.handsPlayed = 0;
            chips = 0;
            bigBlinds = 0;
            preflopCalls = 0;
            preflopBetsAndRaises = 0;
            postflopBetsAndRaises = 0;
            postflopCallsChecksAndFolds = 0;
        }

        /// <summary>
        /// Calculates and returns the VPIP of a given player
        /// </summary>
        /// <returns></returns>
        public int CalculateVPIP()
        {
            if (handsPlayed == 0)
            {
                return 0;
            }
            else
            {
                int VPIP = (preflopCalls + preflopBetsAndRaises) * 100 / handsPlayed;
                return VPIP;
            }
        }

        /// <summary>
        /// Calculates and returns the PFR of a given player
        /// </summary>
        /// <returns></returns>
        public int CalculatePFR()
        {
            if (handsPlayed == 0)
            {
                return 0;
            }
            else
            {
                int PFR = Convert.ToInt16((((decimal)preflopBetsAndRaises) / (decimal)handsPlayed) * 100);
                return PFR;
            }
        }

        /// <summary>
        /// Calculates and returns the AF of a given player
        /// </summary>
        /// <returns></returns>
        public int CalculateAFq()
        {
            if (postflopBetsAndRaises + postflopCallsChecksAndFolds == 0)
            {
                return 0;
            }
            else
            {
                float AFq = ((float)postflopBetsAndRaises / ((float)postflopBetsAndRaises + (float)postflopCallsChecksAndFolds)) * 100;
                return Convert.ToInt16(AFq);
            }
        }

        public double calculateBB()
        {
            return Math.Round(bigBlinds, 1);
        }

        //Combines the current dataset with the dataset in the database
        public void combinethisPlayerWithStoredStats()
        {
            DBControls.CombineDataSets(this);
        }

        /// <summary>
        /// Updates or creates this player in the database
        /// </summary>
        public void UpdateOrCreatePlayerInDatabase()
        {
            DBControls.InsertOrReplacePlayer(this);
        }

        public void PrintStats()
        {
            Console.WriteLine("hp: " + handsPlayed +
                " preC: " + preflopCalls +
                " preBR: " + preflopBetsAndRaises +
                " postBR: " + postflopBetsAndRaises +
                " postCCF: " + postflopCallsChecksAndFolds +
                " Name: " + name);
        }
    }
}
