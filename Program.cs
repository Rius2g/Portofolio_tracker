using System.Globalization;
using System;
using System.Data.SqlClient;

namespace ConsoleApplication
{
    class Program
    {
        static void Main()
        {
        Console.WriteLine("Press any key to continue...");

        Functions f = new Functions();
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
                f.Exit();
                return;
            default:
                Console.WriteLine("Invalid option");
                break;
            }
        }
    }

    class security
    {
        public int type;
        public string ticker;

        public string name;

        public int holdings;
    }
             

    public class Functions
    {

        public int Menu()
        {
            Console.WriteLine("Select your wanted option");
            Console.WriteLine("1. Add security");
            Console.WriteLine("2. Update security");
            Console.WriteLine("3. Remove security");
            Console.WriteLine("4. Display portofolio");
            Console.WriteLine("5. Exit");
            int option = Convert.ToInt32(Console.ReadLine());
            return option;
        }
        public void AddSecurity()
        { //adds a securiy to the database/portofolio
            security sec = new security();
            Console.WriteLine("Adding security");

            //type (crpyto, stock, bond, etc.)
            //Ticker
            //Holdings
            //The program does the rest
            Console.WriteLine("Enter the type of security you want to add\n 1. Crypto\n 2. Stock\n 3. Bond\n 4. ETF\n 5. Mutual Fund\n 6. Index Fund\n");
            int type = Convert.ToInt32(Console.ReadLine());
            if (type < 1 || type > 6)
            {
                throw new Exception("Invalid type");
            }
            else
            {
                Console.WriteLine("Enter the ticker");
                sec.ticker = Console.ReadLine();
                sec.type = type;
                Console.WriteLine("Enter the holdings");
                sec.holdings = Convert.ToInt32(Console.ReadLine());
                
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

        public int calculateTotal()
        { //calculates the total value of the portofolio
            int total = 0;
            //get securities from database
            //if date of fetch is not todays date, fetch new data
            //calculate total value
            //return total
            return total;
        }

        public void DisplayPortofolio()
        { //displays the portofolio

            bool display = true;
            while (display)
            {
                Console.WriteLine("Display portofolio");
            }
        }
    }
}



   


