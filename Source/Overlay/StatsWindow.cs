using System;
using System.Drawing;
using System.Windows.Forms;

namespace easyPokerHUD
{
    public partial class StatsWindow : UserControl
    {
        public int seatNumber;

        public StatsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populates the stats window with the proper values
        /// </summary>
        /// <param name="player"></param>
        public void PopulateStatsWindow(Player player)
        {
            PopulateLabels(player);

            //handsplayed.Text = player.name + "  "+ player.seat; //This line is for debugging purposes only
            this.Username.ForeColor = Color.GhostWhite;
            SetAFqColor();
            SetPFRColor();
            SetVPIPColor();
            SetBBColor();
        }

        /// <summary>
        /// Populates the labels with the player data
        /// </summary>
        /// <param name="player"></param>
        private void PopulateLabels(Player player)
        {
            Username.Text = player.name;// + "(" + getNumberOfHandsPlayedString(player.handsPlayed) + ")";

            //Set VPIP, PFR, AFq and handsPlayed 
            VPIP.Text = player.CalculateVPIP().ToString();
            PFR.Text = player.CalculatePFR().ToString();
            AFq.Text = player.CalculateAFq().ToString();
            BB.Text = player.CalculateBB().ToString() + " BB";
            //handsplayed.Text = getNumberOfHandsPlayedString(player.handsPlayed);


            //In case pfr is higher than vpip, vpip will be shown instead of pfr
            PFR.Text = Int32.Parse(PFR.Text) > Int32.Parse(VPIP.Text)
                ? VPIP.Text
                : PFR.Text;
        }

        /// <summary>
        /// Adjusts the displaying of the number of hands played according to size
        /// </summary>
        /// <param name="numberOfHandsPlayed"></param>
        /// <returns></returns>
        private string GetNumberOfHandsPlayedString(int numberOfHandsPlayed)
        {
            if (numberOfHandsPlayed >= 1000 && numberOfHandsPlayed < 1_000_000)
            {
                return ((float)numberOfHandsPlayed / 1000).ToString("N1") + "k";
            }
            else if (numberOfHandsPlayed >= 1_000_000)
            {
                return ((float)numberOfHandsPlayed / 1_000_000).ToString("N1") + "m";
            }
            else
            {
                return numberOfHandsPlayed.ToString();
            }
        }

        /// <summary>
        /// Sets the color of the VPIP stat
        /// </summary>
        private void SetVPIPColor()
        {
            int vpip = Convert.ToInt16(VPIP.Text);
            if (vpip > 30)
            {
                VPIP.ForeColor = Color.Green;
            }
            else if (vpip > 18)
            {
                VPIP.ForeColor = Color.Orange;
            }
            else if (vpip > 1)
            {
                VPIP.ForeColor = Color.Red;
            }
            else
            {
                VPIP.ForeColor = Color.GhostWhite;
            }
        }

        /// <summary>
        /// Sets the color of the PFR stat
        /// </summary>
        private void SetPFRColor()
        {
            int pfr = Convert.ToInt16(PFR.Text);
            int vpip = Convert.ToInt16(VPIP.Text);

            if (pfr == 0)
            {
                PFR.ForeColor = Color.GhostWhite;
            }
            else if (pfr >= vpip * 0.8)
            {
                PFR.ForeColor = Color.Red;
            }
            else if (pfr > vpip * 0.5)
            {
                PFR.ForeColor = Color.Orange;
            }
            else if (pfr > 0)
            {
                PFR.ForeColor = Color.Green;
            }
        }

        /// <summary>
        /// Sets the color of the AF stat
        /// </summary>
        private void SetAFqColor()
        {
            int afq = Convert.ToInt16(AFq.Text);
            if (afq > 39)
            {
                AFq.ForeColor = Color.Red;
            }
            else if (afq > 24)
            {
                AFq.ForeColor = Color.Orange;
            }
            else if (afq > 1)
            {
                AFq.ForeColor = Color.Green;
            }
            else
            {
                AFq.ForeColor = Color.GhostWhite;
            }
        }

        //Sets the color of the BB stat
        private void SetBBColor()
        {
            string s = BB.Text;
            var firstWhitespace = s.IndexOf(" ");
            string bbString = s.Substring(0, firstWhitespace);
            double bb = Convert.ToDouble(bbString);
            if (bb > 50)
            {
                this.BB.ForeColor = Color.Green;
            }
            else if (bb > 30)
            {
                this.BB.ForeColor = Color.Orange;
            }
            else if (bb > 20)
            {
                this.BB.ForeColor = Color.Yellow;
            }
            else
            {
                this.BB.ForeColor = Color.Red;
            }
        }

        private void StatsWindow_SizeChanged(object sender, EventArgs e)
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;

            while (Username.Width < System.Windows.Forms.TextRenderer.MeasureText(Username.Text, new Font(Username.Font.FontFamily, Username.Font.Size, Username.Font.Style)).Width)
            {
                Username.Font = new Font(Username.Font.FontFamily, Username.Font.Size - 0.5f, Username.Font.Style);
            }

            while (BB.Width < System.Windows.Forms.TextRenderer.MeasureText(BB.Text, new Font(BB.Font.FontFamily, BB.Font.Size, BB.Font.Style)).Width)
            {
                BB.Font = new Font(BB.Font.FontFamily, BB.Font.Size - 0.5f, BB.Font.Style);
            }

        }
    }
}
