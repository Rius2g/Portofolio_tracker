using System;
using System.Collections.Generic;


namespace API 
{
    class Get
    {
        public string Name { get; set; }
        public string Ticker { get; set; }

        public string Type { get; set; }

        public int GetStockPrice(string ticker)
        {
            //API to get the price of stock price
            return 0;
        }

        public int GetCryptoPrice(string ticker)
        {
            //API to get the price of crypto price
            return 0;
        }

        public int GetBondPrice(string ticker)
        {
            //API to get the price of bond price
            return 0;
        }

        public int GetETFPrice(string ticker)
        {
            //API to get the price of ETF price
            return 0;
        }

        public int GetIndexFundPrice(string ticker)
        {
            //API to get the price of index fund price
            return 0;
        }

        public int GetMutualFundPrice(string ticker)
        {
            //API to get the price of mutual fund price
            return 0;
        }

        

    }
}