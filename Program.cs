﻿using System.Globalization;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace ConsoleApplication
{
    class Program
    {
        static void Main()
        {
        Functions f = new Functions();
        while(true)
        {
        int option = f.Menu();
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
                f.DisplayPortfolio();
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
    }
             
public class Functions
{
        public Database.Database db = new Database.Database();


        public int Menu()
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
    public void AddSecurity()
    { //adds a securiy to the database/portofolio
        Console.WriteLine("Adding security");
            
            //type (crpyto, stock, bond, etc.)
            //Ticker
            //Holdings
            //The program does the rest
        Console.Clear();
        Console.WriteLine("Enter the type of security you want to add\n 1. Crypto\n 2. Stock\n 3. ETF\n 4. Mutual Fund\n 5. Index Fund\n 6. By ISI\n 7. Abort");
        ConsoleKeyInfo key = Console.ReadKey(true);

// Check if the key was Enter
        int type = 0;
        while (true)
        {
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
        Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
    }
}
        }

        public void UpdateSecurity()
        { //updates a securiy to the database/portofolio
            Console.WriteLine("Update security");
        }
        public void RemoveSecurity()
        { //removes a securiy to the database/portofolio
            Console.WriteLine("Remove security");
        }
        public void Exit()
        { //exits the program
            Console.WriteLine("Exiting program");
        }

        public void PurgeDatabase()
        { //purges the database
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

        public void listAllHoldings(List<Modules.DisplayedSecurity> securities, int total)
        {
        Console.WriteLine("\nHoldings:\n");
        Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", "Ticker", "Quantity", "Price", "Value", "Change", "Percentage");
        for (int i = 0; i < securities.Count; i++)
        {
            int value = (int)Math.Round(securities[i].Price * securities[i].Quantity);
            double percentage = (double)value / total * 100;
            string changeString = $"{securities[i].Change}%";
            string priceString = $"${value}";

            // Change the color of the Change and Price columns based on the sign of the change
            if (securities[i].Change >= 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                changeString = $"+{securities[i].Change}%";
                priceString = $"${value.ToString("#;-#;0")}";
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                priceString = $"${value.ToString("+0;-#;0")}";
            }

            Console.WriteLine("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}", securities[i].Ticker, securities[i].Quantity, securities[i].Price, priceString, changeString, Math.Round(percentage, 2) + "%");

            Console.ResetColor();
        }
        Console.WriteLine();
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

        public void displayAllocations(List<Modules.DisplayedSecurity> securities, int total)
        {
            int crypto = 0;
            int stock = 0;
            int etf = 0;
            int mutual = 0;
            int index = 0;

            for (int i = 0; i < securities.Count; i++)
            {
                int value = (int)Math.Round(securities[i].Price * securities[i].Quantity);
                switch (securities[i].Type)
                {
                    case 1:
                        crypto += value;
                        break;
                    case 2:
                        stock += value;
                        break;
                    case 3:
                        etf += value;
                        break;
                    case 4:
                        mutual += value;
                        break;
                    case 5:
                        index += value;
                        break;
                }
            }
            if(total == 0)
            {
                Console.WriteLine("No holdings to display.");
                return;
            }
            crypto = crypto / total * 100;
            stock = stock / total * 100;
            etf = etf / total * 100;
            mutual = mutual / total * 100;
            index = index / total * 100;

            Console.WriteLine($"Crypto: {crypto.ToString("0.00")}%");
            Console.WriteLine($"Stock: {stock.ToString("0.00")}%");
            Console.WriteLine($"ETF: {etf.ToString("0.00")}%");
            Console.WriteLine($"Mutual Fund: {mutual.ToString("0.00")}%");
            Console.WriteLine($"Index Fund: {index.ToString("0.00")}%");
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


    public void DisplayPortfolio()
    {
        const int RefreshIntervalMinutes = 15;

    Stopwatch refreshTimer = new Stopwatch();
    refreshTimer.Start();

    //displays the portfolio
    Console.Clear();
    Console.WriteLine();

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

        double change = calculateChange_percent(securities);
        double priceChange = calculateChange_price(securities);
        Console.ForegroundColor = (change >= 0) ? ConsoleColor.Green : ConsoleColor.Red;
        // Display the portfolio information
        Console.WriteLine($"Total portfolio value: ${totalValue}");
        Console.WriteLine($"Change: {change}% ${Math.Abs(priceChange)}\n");
        Console.ResetColor();

        Console.WriteLine("Displaying portfolio");
        displayAllocations(securities, totalValue);
        listAllHoldings(securities, totalValue);
        Console.SetCursorPosition(Console.WindowWidth - 6, Console.WindowHeight - 1);

        // Print the "EXIT" message
        Console.Write("(EXIT)");

        // Wait for a key to be pressed
        ConsoleKeyInfo key = Console.ReadKey(true);

        // Check if the key was Enter or Escape
        if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Escape)
        {
            break; // Exit the while loop
        }

        Thread.Sleep(100000);
    }

    }
}
}



   


