using System.Globalization;
using System.Diagnostics;
using System.Threading;

namespace ConsoleApplication
{
    class Program
    {
        static async Task Main()
        {
            Functions f = new Functions();
            while (true)
            {
                int option = f.Menu();
                switch (option)
                {
                    case 1:
                        await f.AddSecurity();
                        break;
                    case 2:
                        f.UpdateSecurity();
                        break;
                    case 3:
                        f.RemoveSecurity();
                        break;
                    case 4:
                        await f.DisplayPortfolio();
                        break;
                    case 5:
                        f.Settings();
                        break;
                    case 6:
                        f.Exit();
                        return;
                    default:
                        continue;
                }
            }   
        }

        public class Functions
        {
            public Database.Database db = new Database.Database();
            public API.Get get = new API.Get();

            public int Menu()
            {
                db.createDB();
                Console.Clear();
                Console.WriteLine("Select your wanted option");
                Console.WriteLine("1. Add security");
                Console.WriteLine("2. Update security");
                Console.WriteLine("3. Remove security");
                Console.WriteLine("4. Display portofolio");
                Console.WriteLine("5. Settings");
                Console.WriteLine("6. Exit");

                // Read the key that was pressed
                ConsoleKeyInfo key = Console.ReadKey(true);

                // Check value of the key
                if (char.IsDigit(key.KeyChar))
                {
                    int number = int.Parse(key.KeyChar.ToString());
                    return number;
                }
                else
                {
                    return 0;
                }
            }

        

            public async Task AddSecurity()
            { //adds a securiy to the database/portofolio
                db.GetVantageKey();
                Console.WriteLine("Adding security");
                //type (crpyto, stock, bond, etc.)
                //Ticker
                //Holdings
                //The program does the rest
                Console.Clear();
                Console.WriteLine("Enter the type of security you want to add\n 1. Crypto\n 2. Stock\n 3. ETF\n 4. Mutual Fund\n 5. Index Fund\n 6. By ISI\n 7. Manual input\n 8. Savings account\n 9. Abort");
                ConsoleKeyInfo key = Console.ReadKey(true);

                // Check if the key was Enter
                int type = 0;
            
                type = int.Parse(key.KeyChar.ToString());
                switch(type){
                    case int n when (n>=1 && n<=6):
                        Console.WriteLine("Enter the ticker:");
                        string? ticker = Console.ReadLine();

                        if (string.IsNullOrEmpty(ticker))
                        {
                            Console.WriteLine("Invalid ticker. Please enter a valid ticker.");
                            await AddSecurity();
                            return;
                        }

                        Console.WriteLine("Enter the holdings:");
                        try
                        {   
                            
                            double holdings;
                            double.TryParse(Console.ReadLine()?.Replace(".",","), out holdings);
                            Console.WriteLine(holdings);
                            Modules.Security security = new Modules.Security(ticker, holdings, type);
                            db.AddSecurity(security); //add the security to the database
                            return;
                            
                        }
                        catch (System.Exception)
                        {
                            Console.WriteLine("Invalid holdings. Please enter a valid number.");
                            await AddSecurity();
                            return;
                        }

                    case 7:
                        await Add_manual_Security();
                        return;

                    
                    case 8:
                        await AddSavingsAccount();
                        return;
                    case 9:
                        await Main();
                        return;

                    default :
                        Console.WriteLine("Invalid type. Please enter a number between 1 and 8.");
                        return;
                
                }

            }

            public async Task AddSavingsAccount()
            {
                Console.Clear();
                Console.WriteLine("Adding savings account");

                int type = 8;

                Console.WriteLine("Enter the name of the savings account:");
                string? ticker = Console.ReadLine();

                if (string.IsNullOrEmpty(ticker))
                {
                    Console.WriteLine("Invalid name. Please enter a valid name.");
                    return;
                }

                Console.WriteLine("Enter the currency of the savings account:");
                string? currency = Console.ReadLine();

                if (string.IsNullOrEmpty(currency))
                {
                    Console.WriteLine("Invalid currency. Please enter a valid currency.");
                    return;
                }

                double priceMultiple;

                try
                {   
                    string API_Key = db.GetVantageKey();
                    if (API_Key == "error")
                    {
                        Console.WriteLine("2");
                        Console.WriteLine("No Vantage API key found. Please add one in the settings.");
                        return;
                    }
                    priceMultiple = await get.GetCurrencyExchangeRate("USD", currency, API_Key);
                }
                catch(Exception)
                {
                    Console.WriteLine("Invalid currency. Please enter a valid currency.");
                    return;
                }

                Console.WriteLine("Enter the holdings:");
                int holdings;

                if (!int.TryParse(Console.ReadLine(), out holdings))
                {
                    Console.WriteLine("Invalid holdings. Please enter a valid number.");
                    return;
                }
                holdings = (int)(holdings / priceMultiple);
                Modules.Security security = new Modules.Security(ticker, holdings, type);
                security.Price = 1;
                db.AddSavingsAccount(security); //add the security to the database
            }

            public async Task Add_manual_Security()
            {
                Console.Clear();
                Console.WriteLine("Adding manual security");

                Console.WriteLine("Enter the type of security you want to add\n 1. Crypto\n 2. Stock\n 3. ETF\n 4. Mutual Fund\n 5. Index Fund\n 6. Abort");
                ConsoleKeyInfo key = Console.ReadKey(true);

                // Check if the key was Enter
                int type = 0;

                if (char.IsDigit(key.KeyChar))
                {
                    type = int.Parse(key.KeyChar.ToString());
                    if (type < 1 || type > 6)
                    {
                        Console.WriteLine("Invalid type. Please enter a number between 1 and 6.");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Enter the ticker:");
                        string? ticker = Console.ReadLine();

                        if (string.IsNullOrEmpty(ticker))
                        {
                            Console.WriteLine("Invalid ticker. Please enter a valid ticker.");
                            return;
                        }

                        Console.WriteLine("Enter preferred currency: ");
                        string? currency = Console.ReadLine();

                        if (string.IsNullOrEmpty(currency))
                        {
                            Console.WriteLine("Invalid currency. Please enter a valid currency.");
                            return;
                        }
                        double priceMultiple;

                        try
                        {
                            string API_Key = db.GetVantageKey();
                            if (API_Key == "error")
                            {
                                Console.WriteLine("1");
                                Console.WriteLine("No Vantage API key found. Please add one in the settings.");
                                return;
                            }
                            priceMultiple = await get.GetCurrencyExchangeRate("USD", currency, API_Key);
                        }
                        catch(Exception)
                        {
                            Console.WriteLine("Invalid currency. Please enter a valid currency.");
                            return;
                        }

                        Console.WriteLine("Enter the holdings:");
                        int holdings;
                        if (!int.TryParse(Console.ReadLine(), out holdings))
                        {
                            Console.WriteLine("Invalid holdings. Please enter a valid number.");
                            return;
                        }

                        Console.WriteLine("Enter the price:");
                        float price;
                        if (!float.TryParse(Console.ReadLine(), out price))
                        {
                            Console.WriteLine("Invalid price. Please enter a valid number.");
                            return;
                        }


                        Modules.Security security = new Modules.Security(ticker, holdings, type);
                        Console.WriteLine(ticker);
                        Console.WriteLine(holdings);
                        Console.WriteLine(type);
                        Console.WriteLine(priceMultiple);
                        security.Price = price / (float)priceMultiple;
                        db.Add_manual_Security(security); //add the security to the database
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                }
            }

            public void UpdateSecurity()
            {
                Console.Clear();
                Console.WriteLine("Update security: Enter ticker");
                string? ticker = Console.ReadLine();
                if(string.IsNullOrEmpty(ticker))
                {
                    Console.WriteLine("Invalid ticker. Please enter a valid ticker.");
                    return;
                }
                Console.WriteLine("What do you want to update?\n 1. Holdings\n 2. Price\n 3. Both");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (char.IsDigit(key.KeyChar))
                {
                    int option = int.Parse(key.KeyChar.ToString());
                    if(option == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("Enter the new holdings:");
                        int holdings;
                        if (!int.TryParse(Console.ReadLine(), out holdings))
                        {
                            Console.WriteLine("Invalid holdings. Please enter a valid number.");
                            return;
                        }
                        db.UpdateHoldings(ticker, holdings);
                    }
                    else if(option == 2)
                    {
                        Console.Clear();
                        Console.WriteLine("Enter the new price:");
                        decimal price;
                        if (!decimal.TryParse(Console.ReadLine(), out price))
                        {
                            Console.WriteLine("Invalid price. Please enter a valid number.");
                            return;
                        }
                        db.UpdatePrice(ticker, price);
                    }
                    else if(option == 3)
                    {
                        Console.Clear();
                        Console.WriteLine("Enter the new holdings:");
                        int holdings;
                        if (!int.TryParse(Console.ReadLine(), out holdings))
                        {
                            Console.WriteLine("Invalid holdings. Please enter a valid number.");
                            return;
                        }
                        Console.WriteLine("Enter the new price:");
                        decimal price;
                        if (!decimal.TryParse(Console.ReadLine(), out price))
                        {
                            Console.WriteLine("Invalid price. Please enter a valid number.");
                            return;
                        }
                        db.UpdateHoldings(ticker, holdings);
                        db.UpdatePrice(ticker, price);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 3.");
                        return;
                    }
                }
            }
            public void RemoveSecurity()
            { //removes a securiy to the database/portofolio
            //easy db stuff
                Console.Clear();
                Console.WriteLine("Remove security: Enter ticker");
                string? ticker = Console.ReadLine();
                if(string.IsNullOrEmpty(ticker))
                {
                    Console.WriteLine("Invalid ticker. Please enter a valid ticker.");
                    return;
                }

                db.RemoveSecurity(ticker);
            }
            public void Exit()
            { //exits the program
                Console.WriteLine("Exiting program");
            }

            public void Settings()
            {
                Console.Clear();
                Console.WriteLine("Settings");
                Console.WriteLine("1. Change API key");
                Console.WriteLine("2. Change currency");
                Console.WriteLine("3. Purge database");
                Console.WriteLine("4. Back");

                int type = 0;
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (char.IsDigit(key.KeyChar))
                {
                type = int.Parse(key.KeyChar.ToString());

                switch(type){
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Enter new API key:");
                        
                        string ? ApiKey = Console.ReadLine();
                        if(string.IsNullOrEmpty(ApiKey))
                        {
                            Console.WriteLine("Invalid API key. Please enter a valid API key.");
                            return;
                        }
                        db.postVantageKey(ApiKey);
                        Settings();
                        return;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("Not implemented yet.");
                        Settings();
                        return;
                    
                    case 3:
                        PurgeDatabase();
                        Settings();
                        return;

                    
                    case 4:
                        Console.Clear();
                        Console.WriteLine("Going back");
                        return;

                    default:
                        Console.WriteLine("Invalid type. Please enter a number between 1 and 3.");
                        break;
                    
                };
                }
            }
            public void PurgeDatabase()
            { //purges the database
                Console.Clear();
                Console.WriteLine("Purging database");

                db.PurgeDatabase();

                Console.Clear();
                Console.WriteLine("Database Purged");
                Console.WriteLine("Click any key to go back");
                while(true){
                    if(Console.ReadKey(true).Key != ConsoleKey.NoName){
                        break;
                    }
                }

                return;
            }

            public int calculateTotal(List<Modules.DisplayedSecurity> securities)
            { //calculates the total value of the portofolio
                double total = 0;
                for (int i = 0; i < securities.Count; i++)
                {
                    total += securities[i].Price * securities[i].Quantity;
                }
                return (int)Math.Round(total);

            }

            public void listAllHoldings(List<Modules.DisplayedSecurity> securities, double priceInCurrency, string currencyCode)
            {
                CultureInfo cultureInfo = CultureInfo.GetCultureInfo(currencyCode);

                Console.WriteLine($"\nHoldings displayed in {currencyCode}:\n");
                Console.WriteLine("{0,-10} {1,-10} {2,-15} {3,-15} {4,-10} {5,-10} {6,-10}", "Ticker", "Quantity", "Price", "Value", "Change", "Percentage", "Average Purchase Price");

                double totalValue = calculateTotal(securities) * priceInCurrency;

                for (int i = 0; i < securities.Count; i++)
                {
                    double value = (double)securities[i].Price * securities[i].Quantity / priceInCurrency;
                    double price = Math.Round((double)securities[i].Price * priceInCurrency, 2);
                    string priceS = string.Format(cultureInfo, "{0:C}", price);
                    double valueInCurrency = priceInCurrency * (double)securities[i].Quantity * (double)securities[i].Price;
                    string valueS = string.Format(cultureInfo, "{0:# ### ###.00}", valueInCurrency);
                    double percentage = valueInCurrency / totalValue * 100;
                    double change = (double)securities[i].Change;
                    string changeString;
                    string averagePurchasePrice = string.Format(cultureInfo, "{0:C}", securities[i].avgPurchasePrice);

                    // Change the color of the Change and Price columns based on the sign of the change
                    if (change >= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        changeString = $"+{change.ToString("N2")}%";
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        changeString = $"{change.ToString("N2")}%";
                    }

                    Console.WriteLine("{0,-10} {1,-10} {2,-15} {3,-15} {4,-10} {5,-10} {6,-10}", securities[i].Ticker, securities[i].Quantity, priceS, valueS, changeString, Math.Round(percentage, 2) + "%", averagePurchasePrice);

                    Console.ResetColor();
                }
            }



            public string returnType(int type)
            { //returns the type of security
                switch (type)
                {
                    case 1:
                        return "Crypto";
                    case 2:
                        return "Stock";
                    case 3:
                        return "ETF";
                    case 4:
                        return "Mutual Fund";
                    case 5:
                        return "Index Fund";
                    default:
                        return "Invalid type";
                }
            }

            public void DisplayAllocations(List<Modules.DisplayedSecurity> securities, int total)
            {
                Dictionary<int, int> allocation = new Dictionary<int, int>();

                foreach (var security in securities)
                {
                    int value = (int)Math.Round(security.Price * security.Quantity);

                    if (allocation.ContainsKey(security.Type))
                    {
                        allocation[security.Type] += value;
                    }
                    else
                    {
                        allocation.Add(security.Type, value);
                    }
                }

                if (total == 0)
                {
                    Console.WriteLine("No holdings to display.");
                    return;
                }

                Dictionary<int, double> allocationPct = new Dictionary<int, double>();
                foreach (var item in allocation)
                {
                    double pct = (double)item.Value / total * 100;
                    allocationPct.Add(item.Key, pct);
                }

                Console.WriteLine($"Crypto: {allocationPct.GetValueOrDefault(1, 0):0.00}%");
                Console.WriteLine($"Stock: {allocationPct.GetValueOrDefault(2, 0):0.00}%");
                Console.WriteLine($"ETF: {allocationPct.GetValueOrDefault(3, 0):0.00}%");
                Console.WriteLine($"Mutual Fund: {allocationPct.GetValueOrDefault(4, 0):0.00}%");
                Console.WriteLine($"Index Fund: {allocationPct.GetValueOrDefault(5, 0):0.00}%");
                Console.WriteLine($"Savings account: {allocationPct.GetValueOrDefault(8, 0):0.00}%");
            }

            public double calculateChange_percent(List<Modules.DisplayedSecurity> securities)
            {
                // Calculate the total value of the portfolio
                double totalValue = calculateTotal(securities);

                // Calculate the total change for the portfolio based on each security's change and percentage of the portfolio
                double totalChange = 0;
                foreach (var security in securities)
                {
                    double value = security.Price * security.Quantity;
                    double percentage = value / totalValue;
                    totalChange += percentage * security.Change;
                }

                // Return the total change as a percentage
                return Math.Round(totalChange, 2);
            }

            public double calculateChange_price(List<Modules.DisplayedSecurity> securities)
            {
                double totalValue = calculateTotal(securities);
                double totalChange = 0;

                foreach (var security in securities)
                {
                    double value = security.Price * security.Quantity;
                    double percentage = value / totalValue;
                    totalChange += percentage * security.Change * security.Price;
                }

                return Math.Round(totalChange, 2);
            }

            public double averageTotal(List<Modules.DisplayedSecurity> securities)
            {
                double total = 0;
                foreach (var security in securities)
                {
                    total += security.avgPurchasePrice * security.Quantity;
                }

                return total;
            }

            public double averageChange(int total, double averageTotal)
            {
                return Math.Round((total - averageTotal) / averageTotal * 100, 2);
            }


            public async Task DisplayPortfolio()
            {
                const int RefreshIntervalMinutes = 15;
                Stopwatch refreshTimer = new Stopwatch();
                refreshTimer.Start();
                Console.Clear();
                // Get the display currency from user input
                Console.WriteLine("Enter the currency you would like to display the portfolio in: ");
                string? currencyCode = Console.ReadLine();
                string API_Key = db.GetVantageKey();
                if (API_Key == "error")
                {   
                    Console.Clear();
                    Console.WriteLine("3");
                    Console.WriteLine("No Vantage API key found. Please add one in the settings.");
                    Console.WriteLine("Press any key to continue to main menu...");
                    while(true){
                        if(Console.ReadKey(true).Key != ConsoleKey.NoName){
                            break;
                        }
                    }
                    return;

                }
                double currencyRate = await get.GetCurrencyExchangeRate("USD", currencyCode, API_Key);
                db.updatePrices(); 
                Console.WriteLine("Updating data...");
                Thread.Sleep(3000);

                while (true)
                {
                    // Check if it's time to refresh the prices
                    if (refreshTimer.Elapsed.TotalMinutes >= RefreshIntervalMinutes)
                    {
                        db.updatePrices();
                        refreshTimer.Restart();
                    }

                    Console.Clear();

                    // Get the updated securities and total value
                    List<Modules.DisplayedSecurity> securities = db.GetDisplayedSecurities();
                    int totalValue = calculateTotal(securities);

                    // Convert the total value to the selected currency
                    double totalValueInCurrency = totalValue * currencyRate;

                    double change = calculateChange_percent(securities);
                    double priceChange = Convert.ToDouble(calculateChange_price(securities)) * Convert.ToDouble(currencyRate);
                    Console.ForegroundColor = (change >= 0) ? ConsoleColor.Green : ConsoleColor.Red;
                    double avgTotal = averageTotal(securities);
                    double avgChange = averageChange(totalValue, avgTotal);
                    // Display the portfolio information
                    Console.WriteLine($"Total portfolio value: {currencyCode} {totalValueInCurrency.ToString("N2")}");
                    Console.WriteLine($"24H Change: {change}% {currencyCode} {Math.Abs(priceChange):0.00}");
                    Console.WriteLine($"Change from average buy price: {avgChange}% "); //displays change from average price

                    Console.ResetColor();

                    Console.WriteLine("Displaying portfolio");
                    DisplayAllocations(securities, totalValue);
                    listAllHoldings(securities, currencyRate, currencyCode);
                    Console.SetCursorPosition(Console.WindowWidth - 6, Console.WindowHeight - 1);

                    // Print the "EXIT" message
                    Console.Write("(EXIT)");

                    // Wait for a key to be pressed
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    // Check if the key is intended for the DisplayPortfolio function
                    if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.X || key.Key  == ConsoleKey.Enter)
                    {
                        Console.WriteLine("HEIA");
                        // Exit the DisplayPortfolio function
                        Console.WriteLine("Exiting display portfolio view");
                        break;
                    }
                }
            }
        }
    }
}  



   


