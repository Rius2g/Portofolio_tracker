using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Globalization;


namespace Database //do all the setup and functions for database
{

    public class Database
    {
        private API.Get api;
        public string lastFetch;
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
                createTableCommand.CommandText = "CREATE TABLE IF NOT EXISTS Securities (Ticker TEXT, Name TEXT, Price REAL, Quantity INTEGER, Change REAL, Date TEXT, Time TEXT, Type INTEGER)";
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
            double change = 0;
            string name = "";
            DateTime date = DateTime.Now;
            DateTime time = DateTime.Now;

            //fetch the price of the security
            if (security.Type == 1) //crypto
            {
                var result = await api.GetCryptoPriceAndPercentChange(security.Ticker);
                name = result.Item1;
                price = result.Item2;
                change = result.Item3;
            }
            else if (security.Type == 2 || security.Type == 3) //stock, etf, index fund
            {
                var result = await api.GetStockPriceAndPercentChange(security.Ticker);
                name = result.Item1;
                price = result.Item2;
                change = result.Item3;
            }
            else if (security.Type == 5) //mutual fund
            {
                var result = await api.GetMutualFundPrice(security.Ticker);
                name = result.Item1;
                price = result.Item2;
                change = result.Item3;
            }
            else if (security.Type == 6) //by ISIN
            {
                var result = await api.GetStockPriceAndPercentChangeByISIN(security.Ticker);
                name = result.Item1;
                price = result.Item2;
                change = result.Item3;
            }

            // Convert the change to a string with two decimal places, and replace the comma with a period:
            string changeString = change.ToString("0.00", CultureInfo.InvariantCulture).Replace(',', '.');

            // Try to parse the change value with the correct format:
            if (double.TryParse(changeString, NumberStyles.Any, CultureInfo.InvariantCulture, out double changeValue))
            {
                change = changeValue;
            }

            // Convert the price to a string, and replace the comma with a period:
            string priceString = price.ToString("0.00", CultureInfo.InvariantCulture).Replace(',', '.');

            // Open the connection:
            connection.Open();

            // Insert some data:
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Securities (Ticker, Name, Price, Quantity, Change, Date, Time, Type) VALUES ('" + security.Ticker + "', '" + name + "', '" + priceString + "', '" + security.Quantity + "', '" + changeString + "', '" + date.ToString("yyyy-MM-dd") + "', '" + time.ToString("HH:mm") + "', '" + security.Type + "')";
            insertCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void Add_manual_Security(Modules.Security security)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Insert some data:
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Securities (Ticker, Name, Price, Quantity, Change, Date, Time, Type) VALUES ('" + security.Ticker + "', '" + "" + "', '" + security.Price + "', '" + security.Quantity + "', '" + 0 + "', '" + 0 + "', '" + 0 + "', '" + security.Type + "')";
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

        public async void UpdateSecurity(Modules.DisplayedSecurity security)
        {
            double price = 0;
            double change = 0;
            if (security.Type == 1) //crypto
            {
                var result = await api.GetCryptoPriceAndPercentChange(security.Ticker);
                price = result.Item2;
                change = result.Item3;
                string priceString = price.ToString();
                price = Convert.ToDouble(priceString);
            }
            else if (security.Type == 2 || security.Type == 3) //stock, etf, index fund
            {
                var result = await api.GetStockPriceAndPercentChange(security.Ticker);
                price = result.Item2;
                change = result.Item3;
                string priceString = price.ToString();
                price = Convert.ToDouble(priceString);
            }
            else if (security.Type == 5) //mutual fund
            {
                var result = await api.GetMutualFundPrice(security.Ticker);
                price = result.Item2;
                change = result.Item3;
                string priceString = price.ToString();
                price = Convert.ToDouble(priceString);
            }

            else if (security.Type == 6) //by ISIN
            {
                var result = await api.GetStockPriceAndPercentChangeByISIN(security.Ticker);
                price = result.Item2;
                change = result.Item3;
                string type = result.Item4;
                security.Type = typeToInt(result.Item4);
                string priceString = price.ToString();
                price = Convert.ToDouble(priceString);
            }

            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();
            
            DateTime date = DateTime.Now;
            DateTime time = DateTime.Now;
            // Update some data:
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "UPDATE Securities SET Price = " + price + ", Quantity = " + security.Quantity + ", Date = '" + date + "', Time = '" + time + "', Type = '" + security.Type + "' WHERE Ticker = '" + security.Ticker + "'";
            updateCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void UpdateSecurities(List<Modules.DisplayedSecurity> securities)
        {
            Console.WriteLine("HEIA");
            foreach (Modules.DisplayedSecurity security in securities)
            {
                if (security.Type != 4)
                {
                UpdateSecurity(security);
                }
                
            }
            return;
        }

        public int typeToInt(string type)
        {
            if (type == "stock")
            {
                return 2;
            }
            else if (type == "crypto")
            {
                return 1;
            }
            else if (type == "etf")
            {
                return 3;
            }
            else if (type == "index fund")
            {
                return 4;
            }
            else if (type == "mutual fund" || type == "Equity - Other")
            {
                return 5;
            }
            else
            {
                return 0;
            }
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
           DateTime time= DateTime.Now;

           lastFetch = time.ToString("yyyy-MM-dd");

            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = "Holdings.db";
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Read some data:
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Ticker, Price, Quantity, Change, Date, Time, Type FROM Securities";
            using (var reader = selectCommand.ExecuteReader())
            {
                List<Modules.DisplayedSecurity> securities = new List<Modules.DisplayedSecurity>();
                while (reader.Read())
                {
                    Modules.DisplayedSecurity security = new Modules.DisplayedSecurity(reader.GetString(0), reader.GetFloat(1), reader.GetInt16(2), reader.GetInt16(6), reader.GetDouble(3));
                    securities.Add(security);
                }
                return securities;
            }
        }

        public void updatePrices()
        {
            Console.Clear();
            List<Modules.DisplayedSecurity> securities = GetDisplayedSecurities();
            Console.WriteLine(securities.Count);
            // UpdateSecurities(securities);
            return;
        }
    }

}