using System;
using System.Collections.Generic;
using System.Linq;


namespace Modules
{
   public class Security
    {
        public string Name { get; set; }
        public string Ticker { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public string Type { get; set; }

        public Security(string name, string ticker, float price, int quantity, DateTime date, DateTime time, string type)
        {
            Name = name;
            Ticker = ticker;
            Price = price;
            Quantity = quantity;
            Date = date;
            Time = time;
            Type = type;
        }

    }
}