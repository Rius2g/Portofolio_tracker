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
        public int Type { get; set; }

        public Security(string name, string ticker, int quantity, int type)
        {
            Name = name;
            Ticker = ticker;
            Quantity = quantity;
            Type = type;
        }

    }
}