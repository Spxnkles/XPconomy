using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;

namespace XPconomy
{
    public class XPconomyDB
    {
        internal XPconomyDB()
        {
            MySqlConnection connection = CreateConnection();
            try
            {
                connection.Open();
                connection.Close();

                CreateCheckSchema();
            }
            catch (MySqlException ex)
            {
                Logger.LogException(ex);

                XPconomy.Instance.UnloadPlugin();
            }
        }

        private static MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection($"SERVER={XPconomy.Instance.Configuration.Instance.DatabaseAdress};DATABASE={XPconomy.Instance.Configuration.Instance.DatabaseName};UID={XPconomy.Instance.Configuration.Instance.DatabaseUsername};PASSWORD={XPconomy.Instance.Configuration.Instance.DatabasePassword};PORT={XPconomy.Instance.Configuration.Instance.DatabasePort};");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        private void CreateCheckSchema()
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SHOW TABLES LIKE 'XPconomyBalances';";

                    object test = command.ExecuteScalar();
                    if (test == null)
                    {
                        Logger.Log("Table not found, creating one now!");
                        command.CommandText =
                            $@"CREATE TABLE `XPconomyBalances`
                            (
                                `SteamID` BIGINT(20) DEFAULT NULL,
                                `Balance` BIGINT(30) DEFAULT NULL
                            ) COLLATE = 'utf8_general_ci' ENGINE = InnoDB;";
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public void CreateNewBalance(ulong SteamID, ulong Balance)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                try
                {
                    MySqlCommand command = new MySqlCommand
                        (
                        $@"INSERT INTO XPconomyBalances (SteamID, Balance)
                        VALUES (@SteamID, @Balance);", connection
                        );

                    command.Parameters.AddWithValue("@SteamID", SteamID);
                    command.Parameters.AddWithValue("@Balance", Balance);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }

        public bool CheckIfExists(ulong SteamID)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                bool DoesExist;
                connection.Open();
                using (MySqlCommand Command = connection.CreateCommand())
                {
                    Command.CommandText = "Select * from XPconomyBalances where SteamID = " + SteamID + ";";
                    object Result = Command.ExecuteNonQuery();
                    using (MySqlDataReader Reader = Command.ExecuteReader())
                    {
                        if (!Reader.HasRows)
                        {
                            DoesExist = false;
                        }
                        else
                        {
                            DoesExist = true;
                        }
                        Reader.Close();
                    }
                }
                connection.Close();
                return DoesExist;
            }
        }

        public void UpdateBalance(ulong SteamID, ulong Balance)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                connection.Open();
                using (MySqlCommand Command = connection.CreateCommand())
                {
                    Command.CommandText = "UPDATE XPconomyBalances SET Balance = " + Balance + " WHERE SteamID = " + SteamID + ";";
                    Command.ExecuteNonQuery();
                }
                connection.Close();
                return;
            }
        }

        public uint GetBalance(ulong SteamID)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                uint Balance = 0;
                connection.Open();
                using (MySqlCommand Command = connection.CreateCommand())
                {
                    Command.CommandText = "Select `Balance` from XPconomyBalances where SteamID = " + SteamID + ";";
                    object result = Command.ExecuteScalar();
                    if (result != null) uint.TryParse(result.ToString(), out Balance);
                }
                connection.Close();
                return Balance;
            }
        }










    }
}
