using System.Globalization;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ConsoleApplication
{
    class Program
    {
    static async Task Main()
    {
    Functions f = new Functions();
    while (true)
    {
        int option = await f.Menu();
        switch (option)
        {
            case 1:
                f.AddSecurity();
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
                f.PurgeDatabase();
                break;
            case 6:
                f.Exit();
                return;
            default:
                Console.WriteLine("Invalid option");
                break;
        }
    }   
    }

public class Functions
{
    public Database.Database db = new Database.Database();
    public API.Get get = new API.Get();

    public async Task<int> Menu()
    {
        Console.Clear();
        Console.WriteLine("Select your wanted option");
        Console.WriteLine("1. Add security");
        Console.WriteLine("2. Update security");
        Console.WriteLine("3. Remove security");
        Console.WriteLine("4. Display portofolio");
        Console.WriteLine("5. Purge database");
        Console.WriteLine("6. Exit");

        // Read the key that was pressed
        ConsoleKeyInfo key = Console.ReadKey(true);

        // Check if the key was Enter
        if (char.IsDigit(key.KeyChar))
        {
            int number = int.Parse(key.KeyChar.ToString());
            return number;
        }
        else
        {
            Console.WriteLine("Invalid option");
            return 6;
        }
    }

    // Rest of the code remains the same

    public void AddSecurity()
    { //adds a securiy to the database/portofolio
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
        while (true)
        {
            if (char.IsDigit(key.KeyChar))
            {
            type = int.Parse(key.KeyChar.ToString());
            if (type < 1 || type >8)
            {
                Console.WriteLine("Invalid type. Please enter a number between 1 and 8.");
                return;
            }
            else 
            {
                if(type == 7)
                {
                    Add_manual_Security();
                    break;
                }
                if(type == 8)
                {
                    AddSavingsAccount();
                    break;
                }
                Console.WriteLine("Enter the ticker:");
                string? ticker = Console.ReadLine();

                if (string.IsNullOrEmpty(ticker))
                {
                    Console.WriteLine("Invalid ticker. Please enter a valid ticker.");
                    continue;
                }

                Console.WriteLine("Enter the holdings:");
                int holdings;
                if (!int.TryParse(Console.ReadLine(), out holdings))
                {
                    Console.WriteLine("Invalid holdings. Please enter a valid number.");
                    continue;
                }

                Modules.Security security = new Modules.Security(ticker, holdings, type);
                db.AddSecurity(security); //add the security to the database
                break;
            }
            }
            else
            {
            Console.WriteLine("Invalid input. Please enter a number between 1 and 8.");
            }
        }
    }

        public void AddSavingsAccount()
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

            Console.WriteLine("Enter the holdings:");
            int holdings;

            if (!int.TryParse(Console.ReadLine(), out holdings))
            {
                Console.WriteLine("Invalid holdings. Please enter a valid number.");
                return;
            }
            
            Modules.Security security = new Modules.Security(ticker, holdings, type);
            security.Price = 1;
            db.AddSavingsAccount(security); //add the security to the database


        }

        public void Add_manual_Security()
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
                    security.Price = price;
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

        public void PurgeDatabase()
        { //purges the database
            Console.Clear();
            Console.WriteLine("Purging database");
            db.PurgeDatabase();
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

    public void listAllHoldings(List<Modules.DisplayedSecurity> securities, decimal priceInCurrency, string currencyCode)
    {
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(currencyCode);

        Console.WriteLine($"\nHoldings displayed in {currencyCode}:\n");
        Console.WriteLine("{0,-10} {1,-10} {2,-15} {3,-15} {4,-10} {5,-10}", "Ticker", "Quantity", "Price", "Value", "Change", "Percentage");

        decimal totalValue = calculateTotal(securities) * priceInCurrency;

        for (int i = 0; i < securities.Count; i++)
        {
            decimal value = (decimal)securities[i].Price * securities[i].Quantity / priceInCurrency;
            decimal price = Math.Round((decimal)securities[i].Price * priceInCurrency, 2);
            string priceS = string.Format(cultureInfo, "{0:C}", price);
            decimal valueInCurrency = priceInCurrency * (decimal)securities[i].Quantity * (decimal)securities[i].Price;
            string valueS = string.Format(cultureInfo, "{0:# ### ###.00}", valueInCurrency);
            decimal percentage = valueInCurrency / totalValue * 100;
            decimal change = (decimal)securities[i].Change;
            string changeString;

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

            Console.WriteLine("{0,-10} {1,-10} {2,-15} {3,-15} {4,-10} {5,-10}", securities[i].Ticker, securities[i].Quantity, priceS, valueS, changeString, Math.Round(percentage, 2) + "%");

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


        public async Task DisplayPortfolio()
        {
            const int RefreshIntervalMinutes = 15;

            Stopwatch refreshTimer = new Stopwatch();
            refreshTimer.Start();
            Console.Clear();
            // Get the display currency from user input
            Console.WriteLine("Enter the currency you would like to display the portfolio in: ");
            string currencyCode = Console.ReadLine();
            decimal currencyRate = await get.GetCurrencyExchangeRate("USD", currencyCode);
            db.updatePrices(); 

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
                decimal totalValueInCurrency = totalValue * currencyRate;

                double change = calculateChange_percent(securities);
                decimal priceChange = Convert.ToDecimal(calculateChange_price(securities)) * Convert.ToDecimal(currencyRate);
                Console.ForegroundColor = (change >= 0) ? ConsoleColor.Green : ConsoleColor.Red;

                // Display the portfolio information
                Console.WriteLine($"Total portfolio value: {currencyCode} {totalValueInCurrency.ToString("N2")}");
                Console.WriteLine($"Change: {change}% {currencyCode} {Math.Abs(priceChange):0.00}\n");

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



   


