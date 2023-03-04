using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;


namespace Database //do all the setup and functions for database
{

    public class Database
    {
        public static void Main(string[] args)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Create a table:
            var createTable = connection.CreateCommand();
            createTable.CommandText = "CREATE TABLE IF NOT EXISTS Securities (Name VARCHAR(255), Ticker VARCHAR(255), Price FLOAT, Quantity INT, Date DATE, Time TIME, Type VARCHAR(255)))";
            createTable.ExecuteNonQuery();
            // Close the connection:
            connection.Close();
        }

        public void AddSecurity(string name, string ticker, float price, int quantity, DateTime date, DateTime time, string type)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Insert some data:
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Securities VALUES ('" + name + "', '" + ticker + "', " + price + ", " + quantity + ", '" + date + "', '" + time + "', '" + type + "')";
            insertCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void DeleteSecurity(string name)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Delete some data:
            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM Securities WHERE Name = '" + name + "'";
            deleteCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void UpdateSecurity(string name, string ticker, float price, int quantity, DateTime date, DateTime time, string type)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Update some data:
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "UPDATE Securities SET Ticker = '" + ticker + "', Price = " + price + ", Quantity = " + quantity + ", Date = '" + date + "', Time = '" + time + "', Type = '" + type + "' WHERE Name = '" + name + "'";
            updateCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }


        public List<Modules.Security> GetSecurities()
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Read some data:
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Name, Ticker, Price, Quantity, Date, Time, Type FROM Securities";
            using (var reader = selectCommand.ExecuteReader())
            {
                List<Modules.Security> securities = new List<Modules.Security>();
                while (reader.Read())
                {
                    Modules.Security security = new Modules.Security(reader.GetString(0), reader.GetString(1), reader.GetFloat(2), reader.GetInt32(3), reader.GetDateTime(4), reader.GetDateTime(5), reader.GetString(6));
                    securities.Add(security);
                }
                return securities;
            }
        }
    }

}