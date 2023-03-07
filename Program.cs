using System.Globalization;
using System;
using System.Data.SqlClient;
using System.Timers;

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
                f.DisplayPortofolio();
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
            Console.WriteLine("Enter the type of security you want to add\n 1. Crypto\n 2. Stock\n 3. ETF\n 4. Mutual Fund\n 5. Index Fund\n");
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
            continue;
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
            //get securities from database
            //if date of fetch is not todays date, fetch new data
            //calculate total value
            //return total
            return (int)Math.Round(total);

        }

       public void listAllHoldings(List<Modules.DisplayedSecurity> securities, int total)
        { //lists all the holdings of the portofolio
        Console.WriteLine("\n");
        for (int i = 0; i < securities.Count; i++)
        {
            int value = (int)Math.Round(securities[i].Price * securities[i].Quantity);
            double percentage = (double)value / total * 100;
            Console.WriteLine($"* Ticker: {securities[i].Ticker}\n* Quantity: {securities[i].Quantity}\n* Price: {securities[i].Price}\n* Value: {value}\n* Percentage: {percentage}%\n* Type: {returnType(securities[i].Type)}\n");
        }
        //get securities from database
        //if date of fetch is not todays date, fetch new data
        //calculate total value
        //return total
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


        public int calculateChange(int oldTotal, int newTotal)
        { //calculates the change in the portofolio
            //get the old total value of the portofolio
            //get the new total value of the portofolio
            //calculate the change
            //return the change
            return (newTotal - oldTotal) / oldTotal * 100;
        }

        private void onTimedEvent(object source, ElapsedEventArgs e)
        { 

            Console.WriteLine("Updating data");
            db.updatePrices();
        }

        public void DisplayPortofolio()
        { //displays the portofolio
            //start a timer if the timer is > 4 hours, refresh the data
            //get the total value of the portofolio
            List<Modules.DisplayedSecurity> OldSecurities = db.GetDisplayedSecurities(); //get old prices before update
            var timer = new System.Timers.Timer(60 * 60 * 1000 / 4); //every 15 minutes
            timer.Elapsed += onTimedEvent;
            timer.Enabled = true;
            Console.Clear();
            List<Modules.DisplayedSecurity> NewSecurities = db.GetDisplayedSecurities(); //get old prices before update
            int newTotal = calculateTotal(NewSecurities);
            int oldTotal = calculateTotal(OldSecurities);
            Console.WriteLine($"Total portfolio value: ${newTotal}");
            Console.WriteLine($"Change: {calculateChange(oldTotal, newTotal)}%");

            Console.WriteLine("Displaying portofolio");
            displayAllocations(NewSecurities, newTotal);
            listAllHoldings(NewSecurities, newTotal);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    // Read the key that was pressed
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    // Check if the key was Enter
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break; // Exit the while loop
                    }
                }

            }
        }
    }
}



   


