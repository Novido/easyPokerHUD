using System;
using System.IO;
using System.Linq;
using System.Timers;

namespace easyPokerHUD
{
    public class HandHistoryWatcher : FileSystemWatcher
    {
        private Timer _directorySearcher;
        public string pMessage = "";
        public string nMessage = "";

        private readonly Environment.SpecialFolder _windowsEnvironmentFolder;
        private readonly string _pokerRoom;
        private readonly string _handHistoryFolder;

        /// <summary>
        /// Enables the filewatcher with specified path
        /// </summary>
        /// <param name="windowsEnvironmentFolder"></param>
        /// <param name="pokerRoom"></param>
        /// <param name="handHistoryFolder"></param>
        public HandHistoryWatcher(Environment.SpecialFolder windowsEnvironmentFolder, string pokerRoom, string handHistoryFolder)
        {
            //Set the global variables
            _windowsEnvironmentFolder = windowsEnvironmentFolder;
            _pokerRoom = pokerRoom;
            _handHistoryFolder = handHistoryFolder;

            //Set the filters for the filewatcher
            NotifyFilter = NotifyFilters.LastWrite;
            Filter = "*.txt";
            IncludeSubdirectories = true;

            //Enable Filewatcher
            EnableFileWatcher();
        }

        /// <summary>
        /// Calls the enableFilewatcher method
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eve"></param>
        private void TryEnablingFileWatcher(object obj, EventArgs eve)
        {
            EnableFileWatcher();
        }

        /// <summary>
        /// Checks, whether a valid path has been found and enables the filewatching
        /// </summary>
        private void EnableFileWatcher()
        {
            Path = GetHandHistoryDirectory();

            //Begin watching.
            if (!string.IsNullOrEmpty(Path))
            {
                StartFileWatcher();
            }
            else
            {
                ShowErrorMessageAndStartdirectorySearcher();
            }
        }

        /// <summary>
        /// Start the file watcher and dispose the directory searcher if necessary
        /// </summary>
        private void StartFileWatcher()
        {
            _directorySearcher?.Dispose();
            EnableRaisingEvents = true;
            pMessage = _pokerRoom;
            nMessage = "";
        }

        /// <summary>
        /// Accesses specified folders and looks for the handhistory
        /// </summary>
        /// <returns></returns>
        private string GetHandHistoryDirectory()
        {
            try
            {
                //Start in the user folder, where the poker room stores the hand history and move on from there
                var startingDirectory = new DirectoryInfo(@Environment.GetFolderPath(_windowsEnvironmentFolder));
                var possibleDirectories = startingDirectory.GetDirectories().Where(s => s.ToString().Contains(_pokerRoom)).OrderByDescending(f => f.LastWriteTime);

                //Take the list of possible directories and return the most recent one, that contains the hand history folder
                foreach (DirectoryInfo possibleDirectory in possibleDirectories)
                {
                    try
                    {
                        var probableDirectory = possibleDirectory.GetDirectories().Single(s => s.ToString().Contains(_handHistoryFolder));
                        return probableDirectory.FullName;
                    }
                    catch { /*Do nothing when no such directory is found */}
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Starts a directory searcher that keeps looking for the directory
        /// </summary>
        private void ShowErrorMessageAndStartdirectorySearcher()
        {
            if (_directorySearcher == null)
            {
                _directorySearcher = new Timer();
                ShowErrorMessageAndStartdirectorySearcher();
                _directorySearcher.Interval += 3000;
                _directorySearcher.Elapsed += TryEnablingFileWatcher;
                _directorySearcher.Start();
                nMessage = _pokerRoom;
            }
        }
    }
}
