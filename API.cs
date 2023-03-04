using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;



namespace API 
{
   public class Get
    {
        static public string API_KEY = "E6TUK887BIOZOONH";
        public async Task<double> GetStockPrice(string ticker)
        {
            var apiUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&apikey={API_KEY}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JObject.Parse(responseContent)["Global Quote"];
            var stockPrice = responseData.Value<double>("05. price");
            return stockPrice;
            //API to get the price of stock price
        }

        public async Task<Double> GetCryptoPrice(string ticker)
        {
            HttpClient client = new HttpClient();
            string url = $"https://api.coinbase.com/v2/prices/{ticker}-USD/spot";
            HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string responseString = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
            return jsonResponse.data.amount;
        }
        else
        {
            throw new Exception($"Failed to fetch crypto price for {ticker} with status code {response.StatusCode}.");
        }
        }

        public async Task<Double> GetMutualFundPrice(string ticker)
        {
            HttpClient client = new HttpClient();

            string url = $"https://finance.yahoo.com/quote/{ticker}";
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            int index = responseBody.IndexOf("data-reactid=\"50\">") + 18;
            string priceString = responseBody.Substring(index, 10);
            priceString = priceString.Replace(",", "");
            if (double.TryParse(priceString, out double price))
            {
                return price;
            }
            else
            {
                throw new Exception("Failed to parse mutual fund price.");
            }
        }
        
    }


}