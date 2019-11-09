using System;
using System.Threading;
using System.Windows.Forms;

namespace easyPokerHUD
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Only put mandatory stuff in this class
        /// </summary>
        private static readonly Mutex _mutex = new Mutex(true, "{8F6F0AC4-B9A1-45fd-B1Y4T-72F04E6BDE8F}");

        [STAThread]
        public static void Main()
        {
            if (_mutex.WaitOne(TimeSpan.Zero, true))
            {
                //Necessary
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                ApplicationUpdater.CheckForUpdatesAndInstallIfNecessary();
                Application.Run(new MainWindow());
                _mutex.ReleaseMutex();
            }
            else
            {
                Environment.Exit(0);
            }
        }
    }
}
