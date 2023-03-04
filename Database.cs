using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;


namespace Database //do all the setup and functions for database
{

    public class Database
    {
        private API.Get api;
        public Database()
        {
            api = new API.Get();
            // Create a new database connection:
            createDB();
        }

        public void createDB()
        {
            // Create a new database connection:
            using (var connection = new SqliteConnection("Data Source=Holdings.db"))
            {
                connection.Open();

                // Create a new command:
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS Securities (Ticker TEXT, Price REAL, Quantity INTEGER, Date TEXT, Time TEXT, Type INTEGER)";
                createTableCommand.ExecuteNonQuery();

                // Close the connection:
                connection.Close();
            }
        }

        public async void AddSecurity(Modules.Security security)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
            double price = 0;
            DateTime date = DateTime.Now;
            DateTime time = DateTime.Now;

            //fetch the price of the security
            if (security.Type == 1) //crypto
            {
                price = await API.Get.GetCryptoPrice(security.Ticker);
                string priceString = price.ToString();
                price = Convert.ToDouble(priceString);
            }
            else if (security.Type == 2 || security.Type == 3 || security.Type == 6) //stock, etf, index fund
            {
                price = await API.Get.GetStockPrice(security.Ticker);
            }
          
            else if (security.Type == 5) //mutual fund
            {
                price = await API.Get.GetMutualFundPrice(security.Ticker);
            }

            // Open the connection:
            connection.Open();
            // Insert some data:
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Securities (Ticker, Price, Quantity, Date, Time, Type) VALUES ('" + security.Ticker + "', " + price.ToString().Replace(",", ".") + ", " + security.Quantity + ", '" + date.ToString("yyyy-MM-dd") + "', '" + time.ToString("HH:mm:ss") + "', '" + security.Type + "')";
            insertCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void DeleteSecurity(string ticker)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Delete some data:
            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM Securities WHERE Ticker = '" + ticker + "'";
            deleteCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void UpdateSecurity(string ticker, float price, int quantity, DateTime date, DateTime time, string type)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Update some data:
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "UPDATE Securities SET Price = " + price + ", Quantity = " + quantity + ", Date = '" + date + "', Time = '" + time + "', Type = '" + type + "' WHERE Ticker = '" + ticker + "'";
            updateCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void NewPrice( DateTime date) //function to update the price of all securities if date is different from last update
        {

            //first fetch the entire list and see if any are outdated 
            List<Modules.Security> securities = GetSecurities();
            foreach (Modules.Security security in securities)
            {
                if (security.Date != date)
                {
                    //fetch the price of the security
                    double price = 0;
                    if (security.Type == 1) //crypto
                    {
                        price = API.Get.GetCryptoPrice(security.Ticker).Result;
                        string priceString = price.ToString();
                        price = Convert.ToDouble(priceString);
                    }
                    else if (security.Type == 2 || security.Type == 3 || security.Type == 6) //stock, etf, index fund
                    price = API.Get.GetStockPrice(security.Ticker).Result;
                    else if (security.Type == 5) //mutual fund
                    price = API.Get.GetMutualFundPrice(security.Ticker).Result;

                    //update the price
                    UpdateSecurity(security.Ticker, (float)price, security.Quantity, date, DateTime.Now, security.Type.ToString());
                }
            }
            // Create a new database connection:
        }


        public void PurgeDatabase()
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Delete some data:
            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM Securities";
            deleteCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public List<Modules.DisplayedSecurity> GetDisplayedSecurities()
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Read some data:
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Ticker, Price, Quantity, Date, Time, Type FROM Securities";
            using (var reader = selectCommand.ExecuteReader())
            {
                List<Modules.DisplayedSecurity> securities = new List<Modules.DisplayedSecurity>();
                while (reader.Read())
                {
                    Modules.DisplayedSecurity security = new Modules.DisplayedSecurity(reader.GetString(0), reader.GetFloat(1), reader.GetInt16(2), reader.GetInt16(3));
                    securities.Add(security);
                }
                return securities;
            }
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
            selectCommand.CommandText = "SELECT Ticker, Price, Quantity, Date, Time, Type FROM Securities";
            using (var reader = selectCommand.ExecuteReader())
            {
                List<Modules.Security> securities = new List<Modules.Security>();
                while (reader.Read())
                {
                    Modules.Security security = new Modules.Security(reader.GetString(0), reader.GetInt16(2), reader.GetInt16(5));
                    security.Date = reader.GetDateTime(3);
                    security.Time = reader.GetDateTime(4);
                    security.Price = reader.GetFloat(1);
                    securities.Add(security);
                }
                return securities;
            }
        }
    }

}