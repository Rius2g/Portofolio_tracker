using System.Globalization;
using System;
using System.Data.SqlClient;

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
            int type = Convert.ToInt32(Console.ReadLine());
            if (type < 1 || type > 6)
            {
                throw new Exception("Invalid type");
            }
            else
            {
                Console.WriteLine("Enter the ticker");
                string? ticker =  Console.ReadLine();

                if(string.IsNullOrEmpty(ticker))
                {
                    throw new Exception("Invalid ticker");
                }

                Console.WriteLine("Enter the holdings");
                int holdings = Convert.ToInt32(Console.ReadLine());
                Modules.Security security = new Modules.Security(ticker, holdings, type);
                db.AddSecurity(security); //add the security to the database 
                
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
                Console.WriteLine($"* Ticker: {securities[i].Ticker}\n* Quantity: {securities[i].Quantity}\n* Price: {securities[i].Price}\n* Total: ${value}\n* Percentage: {value / total * 100}%\n");
            }
            //get securities from database
            //if date of fetch is not todays date, fetch new data
            //calculate total value
            //return total
        }

        public double calculateChanges(int oldData, int newData) //needs to be calculated before fetch of new data
        { //calculates the changes of the portofolio
            //get securities from database
            //if date of fetch is not todays date, fetch new data
            //calculate changes in % and $
            //return changes
            if (oldData == 0)
            {
                return 100;
            }
            return (newData - oldData) / oldData * 100;

        }

        public void DisplayPortofolio()
        { //displays the portofolio
            //start a timer if the timer is > 4 hours, refresh the data
            //get the total value of the portofolio
            DateTime now = DateTime.Now;
            List<Modules.DisplayedSecurity> oldSecuritiesData = new List<Modules.DisplayedSecurity>();
            string date = now.ToString("yyyy-MM-dd");
            if(db.lastFetch != date)
            {
                oldSecuritiesData = db.GetDisplayedSecurities();
                db.NewPrice();
            }
            List<Modules.DisplayedSecurity> securities = db.GetDisplayedSecurities();
            Console.Clear();
            int newTotal = calculateTotal(securities);
            int oldTotal = calculateTotal(oldSecuritiesData);
            Console.WriteLine($"Total portfolio value: ${newTotal}");
            Console.WriteLine($"Total portfolio changes: {calculateChanges(oldTotal, newTotal)}%");
            Console.WriteLine("Displaying portofolio");
            listAllHoldings(securities, newTotal);

            //clear the console first and then display the portofolio
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



   


