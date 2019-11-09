using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace easyPokerHUD
{
    public class PokerStarsMain
    {
        public static HandHistoryWatcher handHistoryWatcher;
        public static readonly List<Player> playerCache = new List<Player>();
        public static readonly ConcurrentDictionary<string, string> overlays = new ConcurrentDictionary<string, string>();
        public static readonly ConcurrentDictionary<string, PokerStarsHand> newHandsToBeFetched = new ConcurrentDictionary<string, PokerStarsHand>();

        /// <summary>
        /// Activates the Filewatcher
        /// </summary>
        public static void ActivateFileWatcher()
        {
            handHistoryWatcher = new HandHistoryWatcher(System.Environment.SpecialFolder.LocalApplicationData, "PokerStars", "HandHistory");
            handHistoryWatcher.Changed += GetInformationAndPassItToHUD;
        }

        /// <summary>
        /// Creates a hand, fills it with the information about players and finally passes it on to the hud
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void GetInformationAndPassItToHUD(object source, FileSystemEventArgs e)
        {
            PokerStarsHand hand = new PokerStarsHand(e.FullPath);
            if (CheckIfHandIsValidForHUD(hand.tableSize, hand.handInformation))
            {
                CombineDataSets(hand.players);
                CreateNewOverlayOrStoreInformation(hand);
            }
        }

        /// <summary>
        /// Gets the stats stored in the database
        /// </summary>
        /// <param name="players"></param>
        protected static void CombineDataSets(List<Player> players)
        {
            foreach (Player player in players)
            {
                try
                {
                    Player playerStoredInCache = playerCache.Single(p => p.name.Equals(player.name));
                    player.handsPlayed += playerStoredInCache.handsPlayed;
                    player.preflopCalls += playerStoredInCache.preflopCalls;
                    player.preflopBetsAndRaises += playerStoredInCache.preflopBetsAndRaises;
                    player.postflopBetsAndRaises += playerStoredInCache.postflopBetsAndRaises;
                    player.postflopCallsChecksAndFolds += playerStoredInCache.postflopCallsChecksAndFolds;
                    playerCache[playerCache.IndexOf(playerStoredInCache)] = player;
                }
                catch
                {
                    player.CombinethisPlayerWithStoredStats();
                    playerCache.Add(player);
                }
            }
        }

        /// <summary>
        /// Updates the players in the database
        /// </summary>
        public static void UpdatePlayersInDatabaseFromCache()
        {
            foreach (Player player in playerCache)
            {
                player.UpdateOrCreatePlayerInDatabase();
            }
        }

        /// <summary>
        /// Creates a new overlay or stores the information in a list to be fetched by the overlay timer
        /// </summary>
        /// <param name="hand"></param>
        private static void CreateNewOverlayOrStoreInformation(PokerStarsHand hand)
        {
            if (overlays.ContainsKey(hand.tableName))
            {
                newHandsToBeFetched.TryAdd(hand.tableName, hand);
            }
            else
            {
                newHandsToBeFetched.TryAdd(hand.tableName, hand);
                overlays.TryAdd(hand.tableName, hand.tableName);
                new Thread(() => Application.Run(new PokerStarsOverlay(hand))).Start();
            }
        }

        /// <summary>
        /// Checks whether this hand is eligible to be hudded
        /// </summary>
        /// <param name="tableSize"></param>
        /// <param name="handInformation"></param>
        /// <returns></returns>
        private static bool CheckIfHandIsValidForHUD(int tableSize, string handInformation)
        {
            if (handInformation.Contains("Zoom"))
            {
                return false;
            }

            IEnumerable<int> supportedTableSizes = new[]
            {
                2,
                3,
                // TODO Support 4 players tables
                6,
                9
            };

            return supportedTableSizes.Contains(tableSize);
        }
    }
}
