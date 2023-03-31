using System;
using System.Collections.Generic;
using System.Linq;


namespace Modules
{
   public class Security
    {
        public string Ticker { get; set; }
        public float Price { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public DateTime Time { get; set; }
        public int Type { get; set; }

        public double Change { get; set; }

        public Security(string ticker, double quantity, int type)
        {
            Ticker = ticker;
            Quantity = quantity;
            Type = type;
        }

    }
    public class DisplayedSecurity
    {
        public string Ticker { get; set; }
        public float Price { get; set; }
        public double Quantity { get; set; }
        public int Type { get; set; }

        public double Change { get; set; }

        public bool ManualInput { get; set; }

        public double avgPurchasePrice { get; set; }

        public float PurchasePrice { get; set; }

        public DisplayedSecurity(string ticker, float price, double quantity, int type, double change, bool manualInput)
        {
            Ticker = ticker;
            Price = price;
            Quantity = quantity;
            Type = type;
            Change = change;
            ManualInput = manualInput;
        }
    }
}