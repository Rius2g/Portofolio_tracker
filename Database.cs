using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Globalization;


namespace Database //do all the setup and functions for database
{
    public class Database
    {
        string databasename = "Holdings.sqlite";
        private API.Get api = new API.Get();
        public string lastFetch = "";
        
        public string API_KEY = "error";

        public string currency = "USD";
        
        public void createDB()
        {
            // Create a new database connection:
            using (var connection = new SqliteConnection("Data Source=" + databasename))
            {
                connection.Open();

                // Create a new command:
                var createTableCommand = connection.CreateCommand();
                createTableCommand.CommandText = @"CREATE TABLE IF NOT EXISTS Securities (
                                                    Ticker TEXT,
                                                    Name TEXT, 
                                                    Price REAL, 
                                                    Quantity REAL, 
                                                    Change REAL, 
                                                    Date TEXT, 
                                                    Time TEXT, 
                                                    Type INTEGER, 
                                                    ManualInput BOOLEAN,
                                                    PurchasePrice REAL);

                                                    CREATE TABLE IF NOT EXISTS Users (
                                                    AlphavantageKey TEXT,
                                                    Currency TEXT,
                                                    Goal REAL
                                                    );
                                                ";
                createTableCommand.ExecuteNonQuery();

                // Close the connection:
                connection.Close();
            }
        }

        public async void AddSecurity(Modules.Security security)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
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
                if (CheckVantageKey() == false)
                {
                    return;
                }
                var result = await api.GetStockPriceAndPercentChange(security.Ticker, API_KEY);
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
                if (CheckVantageKey() == false)
                {
                    return;
                }
                var result = await api.GetStockPriceAndPercentChangeByISIN(security.Ticker, API_KEY);
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

            string purchasePriceString = priceString;

            // Open the connection:
            connection.Open();

            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = @"INSERT INTO Securities 
                              (Ticker, Name, Price, Quantity, Change, Date, Time, Type, PurchasePrice, ManualInput) 
                              VALUES 
                              ('" + security.Ticker + "', '" + name + "', '" + priceString + "', '" + security.Quantity + "', '" + changeString + "', '" + date.ToString("yyyy-MM-dd") + "', '" + time.ToString("HH:mm") + "', '" + security.Type + "', '" + purchasePriceString + "', 0)";

            var success =  insertCommand.ExecuteNonQuery();
            if (success < 1)
            {
                Console.WriteLine("Error inserting data");
                
            }
            Console.WriteLine("Data inserted");
            // Close the connection:
            connection.Close();
            return;
        }

        public void Add_manual_Security(Modules.Security security)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Insert some data:
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Securities (Ticker, Name, Price, Quantity, Change, Date, Time, Type, ManualInput) VALUES ('" + security.Ticker + "', '" + "" + "', '" + security.Price + "', '" + security.Quantity + "', '" + 0 + "', '" + 0 + "', '" + 0 + "', '" + security.Type + "' , '" + true + "')";
            insertCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void DeleteSecurity(string ticker)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
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

        public void UpdatePrice(string ticker, decimal newprice)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Update some data:
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "UPDATE Securities SET Price = '" + newprice + "' WHERE Ticker = '" + ticker + "'";
            updateCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void UpdateHoldings(string ticker, int newHoldings)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Update some data:
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Ticker, Price, Quantity, Type, Change, ManualInput, PurchasePrice FROM Securities WHERE Ticker = '" + ticker + "'";
            int count = 0;

            List<Modules.DisplayedSecurity> securities = new List<Modules.DisplayedSecurity>();
            using (var reader = selectCommand.ExecuteReader())
            {
                count++;
                double quant;
                bool parsed;
                while (reader.Read())
                {
                    parsed = double.TryParse(reader.GetString(2).Replace(".",","),out quant);
                    if(parsed == false)
                    {
                        Console.WriteLine("Error parsing quantity of " + reader.GetString(0) + " to double. Setting quantity to 0.");
                        quant = 0;
                        while(true){
                            if(Console.ReadKey(true).Key != ConsoleKey.NoName){
                                break;
                            }
                        }
                    }
                    //do a call here to get the price and change
                    Modules.DisplayedSecurity security = new Modules.DisplayedSecurity(reader.GetString(0), reader.GetFloat(1),quant , reader.GetInt16(3), reader.GetDouble(4), reader.GetBoolean(5));
                    security.PurchasePrice = reader.GetFloat(6);
                    securities.Add(security);
                }
            }
            Modules.Security security1;

            if(count > 1)
            {
                //we have more than one entry for this ticker
                //we need to combine them
                security1 = new Modules.Security(securities[0].Ticker, 0, securities[0].Type);
                foreach(Modules.DisplayedSecurity security in securities)
                {
                    security1.Quantity = newHoldings;
                }
            }
            else
            {
                security1 = new Modules.Security(securities[0].Ticker, securities[0].Quantity, securities[0].Type);
                security1.Quantity = newHoldings;
            }

            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM Securities WHERE Ticker = '" + ticker + "'";
            deleteCommand.ExecuteNonQuery();
            
            Modules.Security security2 = new Modules.Security(security1.Ticker, security1.Quantity, security1.Type);
            AddSecurity(security2);

            // Close the connection:
            connection.Close();
        }

        public async void UpdateSecurity(Modules.DisplayedSecurity security)
        {
            double price = 0;
            double change = 0;
            int type = security.Type;
            switch(type)
            {
                case 1:
                    var result = await api.GetCryptoPriceAndPercentChange(security.Ticker);
                    price = result.Item2;
                    change = result.Item3;
                    break;
                case 2: //case for 2 AND 3
                case 3:
                    if (CheckVantageKey() == false)
                    {
                        return;
                    }
                    var result2 = await api.GetStockPriceAndPercentChange(security.Ticker, API_KEY);
                    price = result2.Item2;
                    change = result2.Item3;
                    break;
                case 5:
                    var result4 = await api.GetMutualFundPrice(security.Ticker);
                    price = result4.Item2;
                    change = result4.Item3;
                    break;
                case 6:
                    if (CheckVantageKey() == false)
                    {
                        return;
                    }
                    var result3 = await api.GetStockPriceAndPercentChangeByISIN(security.Ticker, API_KEY);
                    price = result3.Item2;
                    change = result3.Item3;
                    break;
            }

            string priceString = price.ToString("0.00", CultureInfo.InvariantCulture).Replace(',', '.');
            string changeString = change.ToString("0.00", CultureInfo.InvariantCulture).Replace(',', '.');

            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();
            
            DateTime date = DateTime.Now;
            DateTime time = DateTime.Now;
            // Update some data:
            var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = "UPDATE Securities SET Price = '" + priceString + "', Change = '" + changeString + "', Date = '" + date.ToString("yyyy-MM-dd") + "', Time = '" + time.ToString("HH:mm") + "' WHERE Ticker = '" + security.Ticker + "'";
            updateCommand.ExecuteNonQuery();
            
            // Close the connection:
            connection.Close();

        }

        public void AddSavingsAccount(Modules.Security security)
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Insert some data:
            var insertCommand = connection.CreateCommand();
            insertCommand.CommandText = "INSERT INTO Securities (Ticker, Name, Price, Quantity, Change, Date, Time, Type, ManualInput) VALUES ('" + security.Ticker + "', '" + security.Ticker + "', '" + security.Price + "', '" + security.Quantity + "', '" + 0 + "', '" + 0 + "', '" + 0 + "', '" + security.Type + "' , '" + true + "')";
            insertCommand.ExecuteNonQuery();

            // Close the connection:
            connection.Close();
        }

        public void UpdateSecurities(List<Modules.DisplayedSecurity> securities)
        {
            foreach (Modules.DisplayedSecurity security in securities)
            {
                if (security.ManualInput == false) //not manual input
                {
                    UpdateSecurity(security);
                }
                
            }
            return;
        }

        public void RemoveSecurity(string ticker)
        {
            var ConnectionString = "Data Source="+databasename;
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM Securities WHERE Ticker = '" + ticker + "'";
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public int typeToInt(string type)
        {
            switch(type)
            {
                case "stock":
                    return 2;
                case "crypto":
                    return 1;
                case "etf":
                    return 3;
                case "index fund":
                    return 4;
                case "mutual fund":
                    return 5;
                case "Equity - Other":
                    return 5;
                default:
                    return 0;
            }
        }


        public void PurgeDatabase()
        {
            // Create a new database connection:
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Delete some data:
            var deleteCommand = connection.CreateCommand();
            deleteCommand.CommandText = @"DELETE FROM Securities;
                                            DELETE FROM Users";
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
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Read some data:
            var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = "SELECT Ticker, Price, Quantity, Type, Change, ManualInput, PurchasePrice FROM Securities";
            using (var reader = selectCommand.ExecuteReader())
            {
                List<Modules.DisplayedSecurity> securities = new List<Modules.DisplayedSecurity>();
                double quant;
                bool parsed;
                while (reader.Read())
                {
                    parsed = double.TryParse(reader.GetString(2).Replace(".",","),out quant);
                    if(parsed == false)
                    {
                        Console.WriteLine("Error parsing quantity of " + reader.GetString(0) + " to double. Setting quantity to 0.");
                        quant = 0;
                        while(true){
                            if(Console.ReadKey(true).Key != ConsoleKey.NoName){
                                break;
                            }
                        }
                    }
                    //do a call here to get the price and change
                    Modules.DisplayedSecurity security = new Modules.DisplayedSecurity(reader.GetString(0), reader.GetFloat(1),quant , reader.GetInt16(3), reader.GetDouble(4), reader.GetBoolean(5));
                    security.PurchasePrice = reader.GetFloat(6);
                    securities.Add(security);
                }


                List<string> tickers = new List<string>();
                List<Modules.DisplayedSecurity> returnedSecurities = new List<Modules.DisplayedSecurity>();
                foreach (Modules.DisplayedSecurity security in securities)
                {
                    if(tickers.Contains(security.Ticker) == false)
                    {
                        tickers.Add(security.Ticker);
                        Tuple<double, double> avgPriceAndQuantity = calculateAveagePurchasePrice(securities, security.Ticker);
                        security.avgPurchasePrice = avgPriceAndQuantity.Item1;
                        //also need to add the holdings together
                        security.Quantity = avgPriceAndQuantity.Item2;
                        returnedSecurities.Add(security);
                    }
                }

                //get the avg price of the securities, create a list with the tickers that have been added so no dupliactes no more
                return returnedSecurities;
            }
        }
        public Tuple<double, double> calculateAveagePurchasePrice(List<Modules.DisplayedSecurity> securities, string ticker)
            {
                double totalCost = 0;
                double avgPurchasePrice = 0;
                double totalQuantity = 0;
                for(int i = 0; i < securities.Count; i++)
                {
                    if(securities[i].Ticker == ticker)
                    {
                        totalCost += securities[i].PurchasePrice * securities[i].Quantity; //total price for that security
                        totalQuantity += securities[i].Quantity;
                    }
                }
                //create a list for it here

                for(int j = 0; j < securities.Count; j++)
                {
                    if(securities[j].Ticker == ticker)
                    {
                        double purchasePercent = (securities[j].PurchasePrice * securities[j].Quantity) / totalCost;//total price for that security divided by the quantity
                        avgPurchasePrice += purchasePercent * securities[j].PurchasePrice;
                    }
                }
                return Tuple.Create(avgPurchasePrice, totalQuantity); //returns the average price
            }

        public void updatePrices()
        {
            Console.Clear();
            List<Modules.DisplayedSecurity> securities = GetDisplayedSecurities();
            UpdateSecurities(securities);
            return;
        }

        public void postCurrency(string currency)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Update some data:
            var selectCommand = connection.CreateCommand();
            var updateCommand = connection.CreateCommand();

            //need to first check if currency is already in database
            selectCommand.CommandText = "SELECT * FROM Users";
            var reader = selectCommand.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                //update
                updateCommand.CommandText = "UPDATE Users SET Currency = '" + currency + "'";
            }
            else
            {
                //insert
                updateCommand.CommandText = "INSERT INTO Users (Currency) VALUES ('" + currency + "')";
            }

            updateCommand.ExecuteNonQuery();
            connection.Close();
        }




        public int postVantageKey(string key)
        {
            //post key to database
            var connection = new SqliteConnection("Data Source="+databasename);
            connection.Open();

            var selectCommand = connection.CreateCommand();
            var insertCommand = connection.CreateCommand();
            //need to first check if currency is already in database
            selectCommand.CommandText = "SELECT * FROM Users";
            var reader = selectCommand.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                //update
                insertCommand.CommandText = "UPDATE Users SET AlphavantageKey = '" + key + "'";
            }
            else
            {
                //insert
                insertCommand.CommandText = "INSERT INTO Users (AlphavantageKey) VALUES ('" + key + "')";
            }
            int status = insertCommand.ExecuteNonQuery();
            connection.Close();
            return status;

        }


        public string GetVantageKey()
        {
            try
            {
                var connection = new SqliteConnection("Data Source="+databasename);
                connection.Open();
                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT AlphavantageKey FROM Users;";
                var reader = selectCommand.ExecuteReader();
                reader.Read();
                string key = reader.GetString(0);

                connection.Close();
                API_KEY = key;
                return key;
                
            }
            catch (Exception)
            {
                API_KEY = "error";
                return "error";
                throw;
            }
            //get key from database
        }
        public bool CheckVantageKey()
        {
            if (API_KEY == "error")
            {
                Console.Clear();
                Console.WriteLine("No API key found. Please get your API key from alphavantage.co \n and enter it in settings");
                Console.WriteLine("Press any key to continue");
                //make user press a button to continue
                while(true){
                    if(Console.ReadKey(true).Key != ConsoleKey.NoName){
                        break;
                    }
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public string getCurrency()
        {
            try
            {
                var connection = new SqliteConnection("Data Source="+databasename);
                connection.Open();
                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT Currency FROM Users;";
                var reader = selectCommand.ExecuteReader();
                reader.Read();
                string curr = reader.GetString(0);

                connection.Close();
                currency = curr;
                return curr;
                
            }
            catch (Exception)
            {
                return "USD";
                throw;
            }

        }

        public double getGoal()
        {
            try
            {
                var connection = new SqliteConnection("Data Source="+databasename);
                connection.Open();
                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = "SELECT Goal FROM Users;";
                var reader = selectCommand.ExecuteReader();
                reader.Read();
                double goal = reader.GetDouble(0);

                connection.Close();
                return goal;
                
            }
            catch (Exception)
            {
                return 0;
                throw;
            }
        }

        public void postGoal(double goal)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = databasename;
            var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);

            // Open the connection:
            connection.Open();

            // Update some data:
            var selectCommand = connection.CreateCommand();
            var updateCommand = connection.CreateCommand();

            //need to first check if currency is already in database
            selectCommand.CommandText = "SELECT * FROM Users";
            var reader = selectCommand.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                //update
                updateCommand.CommandText = "UPDATE Users SET Goal = '" + goal + "'";
            }
            else
            {
                //insert
                updateCommand.CommandText = "INSERT INTO Users (Goal) VALUES ('" + goal + "')";
            }

            updateCommand.ExecuteNonQuery();
            connection.Close();
        }
    }

}