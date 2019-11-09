using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace easyPokerHUD
{
    public partial class MainWindow : Form
    {
        private readonly Timer _fileWatcherStatusUpdateTimer = new Timer();

        public MainWindow()
        {
            InitializeComponent();

            //Set the version number label to the current version
            versionNumberLabel.Text = $"v {ApplicationUpdater.GetCurrentProductVersion()}";

            //Start the filewatchers
            MainMethods.ActivateFileWatchers();

            //Start the timer that updates the console
            _fileWatcherStatusUpdateTimer.Tick += UpdateHUDStatus;
            _fileWatcherStatusUpdateTimer.Interval = 1500;
            _fileWatcherStatusUpdateTimer.Start();

            errorMessage.Click += MainMethods.OpenQuickStartGuide;
        }

        /// <summary>
        /// Gathers the current status of every filewatcher and displays it as a label
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public void UpdateHUDStatus(object obj, EventArgs e)
        {
            string[] statusStrings = MainMethods.GetHUDStatusStrings();

            if (statusStrings[0].Equals(""))
            {
                successMessage.Text = "";
            }
            else
            {
                successMessage.Text = "HUD is up and running for " + statusStrings[0] + ".";
            }

            if (statusStrings[1].Equals(""))
            {
                errorMessage.Text = "";
                errorMessage.Hide();
                _fileWatcherStatusUpdateTimer.Stop();
            }
            else
            {
                errorMessage.Text = "Could not find hand histories for " + statusStrings[1] + ". Click here to see how to fix this.";
            }
        }

        /// <summary>
        /// Close every thread and write cached players to database
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //buyMessage.Text = "Writing players to database";
            PokerStarsMain.UpdatePlayersInDatabaseFromCache();
            EightPokerMain.UpdatePlayersInDatabaseFromCache();

            try
            {
                Environment.Exit(Environment.ExitCode);
            }
            catch (Exception p)
            {
                Console.WriteLine("Block 1");
                Console.WriteLine(p.Message);
            }
        }

        /// <summary>
        /// Opens donation page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DonateButton_Click(object sender, EventArgs e)
        {
            Process.Start("https://easypokerhud.com/join-hof/");
        }
    }
}
