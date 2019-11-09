using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows.Forms;

namespace easyPokerHUD
{
    internal static class ApplicationUpdater
    {
        private static readonly string _updateInformationURL = "https://easypokerhud.com/updateInformation.xml";
        private static readonly string _pathToSaveDownload = @Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\easyPokerHUD";

        /// <summary>
        /// Checks for udpates and installs them if necessary
        /// </summary>
        public static void CheckForUpdatesAndInstallIfNecessary()
        {
            string updateInformation = DownloadUpdateInformation();

            if (updateInformation != null && CheckIfThisVersionIsOutdated(updateInformation))
            {
                MessageBox.Show("Update found! easyPokerHUD will now download and install the update");
                DownloadAndExecuteInstaller(updateInformation);
            }
        }

        /// <summary>
        /// Downloads the xml document from easyPokerHUD.com
        /// </summary>
        /// <returns></returns>
        private static string DownloadUpdateInformation()
        {
            try
            {
                return new WebClient().DownloadString(_updateInformationURL);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Checks if the current version is outdated
        /// </summary>
        /// <param name="updateInformation"></param>
        /// <returns></returns>
        private static bool CheckIfThisVersionIsOutdated(string updateInformation)
        {
            var result = GetCurrentProductVersion().CompareTo(GetNewestVersion(updateInformation));

            return result < 0;
        }

        /// <summary>
        /// Gets the current product version
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentProductVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return fileVersionInfo.ProductVersion;
        }

        /// <summary>
        /// Reads out the newest version stored in the xml document
        /// </summary>
        /// <param name="updateInformation"></param>
        /// <returns></returns>
        private static string GetNewestVersion(string updateInformation)
        {
            string version = updateInformation.Substring(updateInformation.IndexOf("<version>") + 9);

            return version.Substring(0, version.IndexOf("</"));
        }

        /// <summary>
        /// Downloads the installer
        /// </summary>
        /// <param name="updateInformation"></param>
        private static void DownloadAndExecuteInstaller(string updateInformation)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(GetDownloadURL(updateInformation), _pathToSaveDownload + @"\easyPokerHUD.exe");
                    Process.Start(_pathToSaveDownload + @"\easyPokerHUD.exe");
                    Environment.Exit(0);
                }
            }
            catch
            {
                MessageBox.Show("Sometimes I'm a tiny helpless program. I couldn't update myself automatically. " +
                    "Please help me by removing your current installation and installing the update manually from www.easypokerhud.com");
            }
        }

        /// <summary>
        /// Reads out the download URL where the update is stored
        /// </summary>
        /// <param name="updateInformation"></param>
        /// <returns></returns>
        private static string GetDownloadURL(string updateInformation)
        {
            string downloadURL = updateInformation.Substring(updateInformation.IndexOf("<url>") + 5);

            return downloadURL.Substring(0, downloadURL.IndexOf("</"));
        }
    }
}
