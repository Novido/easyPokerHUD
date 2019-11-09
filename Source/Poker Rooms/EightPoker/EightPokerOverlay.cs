using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace easyPokerHUD
{
    internal partial class EightPokerOverlay : PokerRoomOverlay
    {
        public EightPokerOverlay(EightPokerHand hand)
        {
            InitializeComponent();

            //Prepare the overlay
            PrepareOverlay(hand.tableName, hand.tableSize, hand.playerName);

            //Add an event to the timers
            statsFetcherTimer.Tick += UpdatePlayerStatsIfNewHandExists;
            controlSizeUpdateTimer.Tick += UpdateControlSizeOrCloseOverlay;

            //Set the size for all controls
            SetPositionOfStatsWindowsWindowsAccordingToTableSize();
        }

        /// <summary>
        /// Updates all of the controls size on the form or closes the overlay if the table is closed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eve"></param>
        public void UpdateControlSizeOrCloseOverlay(object obj, EventArgs eve)
        {
            string windowTableName = GetTableWindowName();
            if (string.IsNullOrEmpty(windowTableName))
            {
                EightPokerMain.overlays.TryRemove(tableName, out tableName);
                Close();
                Thread.CurrentThread.Abort();
            }
            else if (Height != oldHeight || Width != oldWidth)
            {
                oldHeight = Height;
                oldWidth = Width;
                SetPositionOfStatsWindowsWindowsAccordingToTableSize();
                SetStatsWindowsFontSize();
            }
        }

        /// <summary>
        /// Updates the player stats
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eve"></param>
        protected void UpdatePlayerStatsIfNewHandExists(object obj, EventArgs eve)
        {
            if (EightPokerMain.newHandsToBeFetched.TryRemove(tableName, out EightPokerHand hand))
            {
                hand.players = PreparePlayerListForCorrectPositioning(hand.players);
                PopulateStatsWindows(hand.players);
            }
        }

        /// <summary>
        /// Prepares the player seat numbers according to the size of the table
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        protected new List<Player> PreparePlayerListForCorrectPositioning(List<Player> players)
        {
            Dictionary<int, Func<List<Player>, List<Player>>> positionControls = new Dictionary<int, Func<List<Player>, List<Player>>>()
            {
                [2] = PreparePlayerListForHeadsUp,
                [4] = PreparePlayerListFor4Max,
                [6] = PreparePlayerListFor6Max,
                [9] = PreparePlayerListFor9Max,
                [10] = PreparePlayerListFor10Max
            };

            if (positionControls.TryGetValue(tableSize, out Func<List<Player>, List<Player>> method))
            {
                method.Invoke(players);
            }

            return players;
        }

        private List<Player> PreparePlayerListForHeadsUp(List<Player> players)
        {
            while (!players.Find(p => p.name.Equals(playerName)).seat.Equals(4))
            {
                foreach (Player player in players)
                {
                    if (player.seat == 9)
                    {
                        player.seat = 0;
                    }
                    if (player.seat == 4)
                    {
                        player.seat += 3;
                    }
                    player.seat++;
                }
            }
            return players;
        }

        private List<Player> PreparePlayerListFor4Max(List<Player> players)
        {
            while (!players.Find(p => p.name.Equals(playerName)).seat.Equals(4))
            {
                foreach (Player player in players)
                {
                    if (player.seat == 1 || player.seat == 4)
                    {
                        player.seat += 2;
                    }
                    if (player.seat == 7)
                    {
                        player.seat++;
                    }
                    if (player.seat == 9)
                    {
                        player.seat = 0;
                    }
                    player.seat++;
                }
            }
            return players;
        }

        private List<Player> PreparePlayerListFor6Max(List<Player> players)
        {
            while (!players.Find(p => p.name.Equals(playerName)).seat.Equals(4))
            {
                foreach (Player player in players)
                {
                    if (player.seat == 2 || player.seat == 4 || player.seat == 7)
                    {
                        player.seat++;
                    }
                    if (player.seat == 9)
                    {
                        player.seat = 0;
                    }
                    player.seat++;
                }
            }
            return players;
        }

        private List<Player> PreparePlayerListFor9Max(List<Player> players)
        {
            while (!players.Find(p => p.name.Equals(playerName)).seat.Equals(5))
            {
                foreach (Player player in players)
                {
                    if (player.seat == 7)
                    {
                        player.seat++;
                    }
                    if (player.seat == 10)
                    {
                        player.seat = 0;
                    }
                    player.seat++;
                }
            }
            return players;
        }

        private List<Player> PreparePlayerListFor10Max(List<Player> players)
        {
            while (!players.Find(p => p.name.Equals(playerName)).seat.Equals(5))
            {
                foreach (Player player in players)
                {
                    if (player.seat == 10)
                    {
                        player.seat = 0;
                    }
                    player.seat++;
                }
            }
            return players;
        }

        /// <summary>
        /// Updates the size of the controls for the according table size
        /// </summary>
        private void SetPositionOfStatsWindowsWindowsAccordingToTableSize()
        {
            Dictionary<int, Action> positionControls = new Dictionary<int, Action>()
            {
                [2] = PositionControlsHeadsUp,
                [4] = PositionControls4Max,
                [6] = PositionControls6Max,
                [9] = PositionControls9Max,
                [10] = PositionControls10Max
            };

            if (positionControls.TryGetValue(tableSize, out Action action))
            {
                action.Invoke();
            }
        }

        private void PositionControlsHeadsUp()
        {
            statsWindow4.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 2.5), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow9.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.65), Convert.ToInt32(Convert.ToDouble(Height / 8.5)));
        }

        private void PositionControls4Max()
        {
            statsWindow1.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.24), Convert.ToInt32(Convert.ToDouble(Height / 2.05)));
            statsWindow4.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 2.5), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow7.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 35.0), Convert.ToInt32(Convert.ToDouble(Height / 2.05)));
            statsWindow9.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.65), Convert.ToInt32(Convert.ToDouble(Height / 8.5)));
        }

        private void PositionControls6Max()
        {
            statsWindow1.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.34), Convert.ToInt32(Convert.ToDouble(Height / 7.5)));
            statsWindow2.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.24), Convert.ToInt32(Convert.ToDouble(Height / 2.05)));
            statsWindow4.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.82), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow6.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 3.9), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow7.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 35.0), Convert.ToInt32(Convert.ToDouble(Height / 2.05)));
            statsWindow9.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 13.0), Convert.ToInt32(Convert.ToDouble(Height / 7.5)));
        }

        private void PositionControls9Max()
        {
            statsWindow1.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.37), Convert.ToInt32(Convert.ToDouble(Height / 7.5)));
            statsWindow2.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.24), Convert.ToInt32(Convert.ToDouble(Height / 2.7)));
            statsWindow3.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.24), Convert.ToInt32(Convert.ToDouble(Height / 1.79)));
            statsWindow4.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.6), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow5.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 2.47), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow6.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 5.5), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow7.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 35.0), Convert.ToInt32(Convert.ToDouble(Height / 1.79)));
            statsWindow9.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 35.0), Convert.ToInt32(Convert.ToDouble(Height / 2.7)));
            statsWindow10.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 13), Convert.ToInt32(Convert.ToDouble(Height / 7.5)));
        }

        private void PositionControls10Max()
        {
            statsWindow1.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.6), Convert.ToInt32(Convert.ToDouble(Height / 24)));
            statsWindow2.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.24), Convert.ToInt32(Convert.ToDouble(Height / 2.7)));
            statsWindow3.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.24), Convert.ToInt32(Convert.ToDouble(Height / 1.79)));
            statsWindow4.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.6), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow5.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 2.47), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow6.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 5.5), Convert.ToInt32(Convert.ToDouble(Height / 1.305)));
            statsWindow7.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 35.0), Convert.ToInt32(Convert.ToDouble(Height / 1.79)));
            statsWindow8.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 35.0), Convert.ToInt32(Convert.ToDouble(Height / 2.7)));
            statsWindow9.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 5.5), Convert.ToInt32(Convert.ToDouble(Height / 24)));
            statsWindow10.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 2.47), Convert.ToInt32(Convert.ToDouble(Height / 24)));
        }
    }
}
