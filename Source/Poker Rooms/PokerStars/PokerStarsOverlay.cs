using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace easyPokerHUD
{
    internal partial class PokerStarsOverlay : PokerRoomOverlay
    {
        public PokerStarsOverlay(PokerStarsHand hand)
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
            if (string.IsNullOrEmpty(GetTableWindowName()))
            {
                PokerStarsMain.overlays.TryRemove(tableName, out tableName);
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
            if (PokerStarsMain.newHandsToBeFetched.TryRemove(tableName, out PokerStarsHand hand))
            {
                hand.players = PreparePlayerListForCorrectPositioning(hand.players);
                PopulateStatsWindows(hand.players);
            }
        }

        /// <summary>
        /// Updates the size of the controls for the according table size
        /// </summary>
        protected void SetPositionOfStatsWindowsWindowsAccordingToTableSize()
        {
            Dictionary<int, Action> positionControls = new Dictionary<int, Action>()
            {
                [2] = PositionControlsHeadsUp,
                [3] = PositionControls3Max,
                [6] = PositionControls6Max,
                [9] = PositionControls9Max
            };

            if (positionControls.TryGetValue(tableSize, out Action action))
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Positions the controls relative to the window size for HeadsUp
        /// </summary>
        private void PositionControlsHeadsUp()
        {
            statsWindow1.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 2.4), Convert.ToInt32(Convert.ToDouble(Height / 1.25)));
            statsWindow2.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.65), Convert.ToInt32(Convert.ToDouble(Height / 6.5)));
        }

        /// <summary>
        /// Positions the controls relative to the window size for 3max
        /// </summary>
        private void PositionControls3Max()
        {
            statsWindow1.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.25), Convert.ToInt32(Convert.ToDouble(Height / 3.01)));
            statsWindow2.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 2.4), Convert.ToInt32(Convert.ToDouble(Height / 1.25)));
            statsWindow3.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 25.0), Convert.ToInt32(Convert.ToDouble(Height / 3.01)));
        }

        /// <summary>
        /// Positions the controls relative to the window size for 6Max
        /// </summary>
        private void PositionControls6Max()
        {
            statsWindow1.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.25), Convert.ToInt32(Convert.ToDouble(Height / 3.01)));
            statsWindow2.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.25), Convert.ToInt32(Convert.ToDouble(Height / 1.58)));
            statsWindow3.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 2.4), Convert.ToInt32(Convert.ToDouble(Height / 1.25)));
            statsWindow4.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 25.0), Convert.ToInt32(Convert.ToDouble(Height / 1.58)));
            statsWindow5.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 25.0), Convert.ToInt32(Convert.ToDouble(Height / 3.01)));
            statsWindow6.Location = new Point(Convert.ToInt32(Convert.ToDouble(Width) / 1.65), Convert.ToInt32(Convert.ToDouble(Height / 6.5)));
        }

        /// <summary>
        /// Positions the controls relative to the window size for 9Max
        /// </summary>
        private void PositionControls9Max()
        {
            statsWindow1.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 1.40), Convert.ToInt32(Convert.ToDouble(this.Height / 8))); // (1.34, 8.0)     *5th from player
            statsWindow2.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 1.40), Convert.ToInt32(Convert.ToDouble(this.Height / 3.3))); // ()            *6th from player
            statsWindow3.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 1.38), Convert.ToInt32(Convert.ToDouble(this.Height / 2))); // (1.2, 1.9)      *7th from player
            statsWindow4.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 1.45), Convert.ToInt32(Convert.ToDouble(this.Height / 1.42))); // (1.4, 1.37)  *8th from player
            statsWindow5.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 2.4), Convert.ToInt32(Convert.ToDouble(this.Height / 1.28))); // (2.4, 1.25)   *Player
            statsWindow6.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 7.8), Convert.ToInt32(Convert.ToDouble(this.Height / 1.42))); // (7.8, 1.37)   *1st from player
            statsWindow7.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 12.0), Convert.ToInt32(Convert.ToDouble(this.Height / 2))); // (55, 1.9)       *2rd from player
            statsWindow8.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 12.0), Convert.ToInt32(Convert.ToDouble(this.Height / 3.3))); // (14.0, 3.01)  *3nd from player
            statsWindow9.Location = new Point(Convert.ToInt32(Convert.ToDouble(this.Width) / 8), Convert.ToInt32(Convert.ToDouble(this.Height / 8))); // (8.0, 8.0)       *4th from player
        }

        /// <summary>
        /// Needed by the designer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PokerStarsOverlay_Load(object sender, EventArgs e) { }
    }
}
