using System;
using System.Data.SQLite;
using System.IO;

namespace easyPokerHUD
{
    internal static class DBControls
    {
        public static readonly string dataBasePath = @Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\easyPokerHUD\";
        public static readonly string dataBasePathWithFile = dataBasePath + "easyPokerHUD.sqlite";
        public static readonly string connectionInfo = "Data Source=" + dataBasePathWithFile + ";Version=3;";

        /// <summary>
        /// Creates the database
        /// </summary>
        public static void CreateDatabase()
        {
            Directory.CreateDirectory(dataBasePath);
            if (!File.Exists(dataBasePathWithFile))
            {
                SQLiteConnection.CreateFile(dataBasePathWithFile);
            }
            CreateTable("PokerStars");
            CreateTable("EightPoker");
        }

        /// <summary>
        /// Inserts a table with specified name in the database
        /// </summary>
        /// <param name="pokerRoom"></param>
        public static void CreateTable(string pokerRoom)
        {
            string query = "create table if not exists "
                + pokerRoom
                + " ("
                + "name char(30) primary key,"
                + "handsPlayed int,"
                + "preflopCalls int,"
                + "preflopBetsAndRaises int,"
                + "postflopBetsAndRaises int,"
                + "postflopCallsChecksAndFolds int"
                + ")";
            ExecuteCommandInDatabase(query);
        }

        /// <summary>
        /// Inserts or replaces the player
        /// </summary>
        /// <param name="player"></param>
        public static void InsertOrReplacePlayer(Player player)
        {
            string query = "insert or replace into "
                + player.pokerRoom
                + " values ("
                + "'" + player.name + "',"
                + player.handsPlayed + ","
                + player.preflopCalls + ","
                + player.preflopBetsAndRaises + ","
                + player.postflopBetsAndRaises + ","
                + player.postflopCallsChecksAndFolds
                + ")";
            ExecuteCommandInDatabase(query);
        }

        /// <summary>
        /// Reads out the stored data for the specific player and combines it with the current stats
        /// </summary>
        /// <param name="player"></param>
        public static void CombineDataSets(Player player)
        {
            try
            {
                string query = "select * from " + player.pokerRoom + " where name = '" + player.name + "'";
                using (SQLiteConnection connection = new SQLiteConnection(connectionInfo))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                player.name = Convert.ToString(reader["name"]);
                                player.handsPlayed += Convert.ToInt32(reader["handsPlayed"]);
                                player.preflopCalls += Convert.ToInt32(reader["preflopCalls"]);
                                player.preflopBetsAndRaises += Convert.ToInt32(reader["preflopBetsAndRaises"]);
                                player.postflopBetsAndRaises += Convert.ToInt32(reader["postflopBetsAndRaises"]);
                                player.postflopCallsChecksAndFolds += Convert.ToInt32(reader["postflopCallsChecksAndFolds"]);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                CreateDatabase();
            }
        }

        /// <summary>
        /// Executes a query in the database and throws an exception if something goes wrong
        /// </summary>
        /// <param name="query"></param>
        private static void ExecuteCommandInDatabase(string query)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionInfo))
                {
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                CreateDatabase();
            }
        }
    }
}
