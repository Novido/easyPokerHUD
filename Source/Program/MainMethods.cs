using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace easyPokerHUD
{
    internal static class MainMethods
    {
        private static List<string> _positiveMessages = new List<string>();
        private static List<string> _negativeMessages = new List<string>();

        /// <summary>
        /// Checks which pokerRooms are installed and activates their filewatchers
        /// </summary>
        public static void ActivateFileWatchers()
        {
            if (CheckIfPokerRoomIsInstalled("PokerStars"))
            {
                PokerStarsMain.ActivateFileWatcher();
            }
            if (CheckIfPokerRoomIsInstalled("888poker"))
            {
                EightPokerMain.ActivateFileWatcher();
            }
        }

        /// <summary>
        /// Accesses the uninstall list of windows and checks if it contains the pokerroom-name
        /// </summary>
        /// <param name="pokerRoom"></param>
        /// <returns></returns>
        private static bool CheckIfPokerRoomIsInstalled(string pokerRoom)
        {
            const string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        try
                        {
                            string name = (string)subkey.GetValue("DisplayName");
                            if (name.Contains(pokerRoom))
                            {
                                return true;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Opens the QuickStartGuide in a new browser tab
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public static void OpenQuickStartGuide(object obj, EventArgs e)
        {
            Process.Start("https://easypokerhud.com/quickstart-guides/");
        }

        /// <summary>
        /// Adds all status messages to two lists
        /// </summary>
        public static void AddMessagesToLists()
        {
            _positiveMessages = new List<string>();
            _negativeMessages = new List<string>();

            if (PokerStarsMain.handHistoryWatcher != null)
            {
                _positiveMessages.Add(PokerStarsMain.handHistoryWatcher.pMessage);
                _negativeMessages.Add(PokerStarsMain.handHistoryWatcher.nMessage);
            }

            if (EightPokerMain.handHistoryWatcher != null)
            {
                _positiveMessages.Add(EightPokerMain.handHistoryWatcher.pMessage);
                _negativeMessages.Add(EightPokerMain.handHistoryWatcher.nMessage);
            }
        }

        /// <summary>
        /// Builds a success and an error string to display in the main window
        /// </summary>
        /// <returns></returns>
        public static string[] GetHUDStatusStrings()
        {
            //Get all status messages from every FileWatcher class
            AddMessagesToLists();

            //Start with two empty strings and build them into properly formatted positive and negative messages
            string positiveMessage = string.Join(",", _positiveMessages);
            string negativeMessage = string.Join(",", _negativeMessages);

            //Put both status strings into an array and return it
            string[] statusStrings = new string[2];
            statusStrings[0] = positiveMessage;
            statusStrings[1] = negativeMessage;

            return statusStrings;
        }
    }
}
