using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;



namespace API 
{
   public class Get
    {
        static public string API_KEY = "E6TUK887BIOZOONH";
        public async Task<Tuple<double, double>> GetStockPriceAndPercentChange(string ticker)
        {
            var apiUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&apikey={API_KEY}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JObject.Parse(responseContent)["Global Quote"];
            var stockPrice = responseData.Value<double>("05. price");
            var previousClose = responseData.Value<double>("08. previous close");
            var percentChange = (stockPrice - previousClose) / previousClose * 100;
            return Tuple.Create(stockPrice, percentChange);
        }

       public async Task<Tuple<double, double>> GetCryptoPriceAndPercentChange(string ticker)
       {
        HttpClient client = new HttpClient();
        string url = $"https://api.coinbase.com/v2/prices/{ticker}-USD/spot";
        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
        string responseString = await response.Content.ReadAsStringAsync();
        dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
        double currentPrice = jsonResponse.data.amount;
        double previousClose = jsonResponse.data.amount;
        var historicPricesUrl = $"https://api.coinbase.com/v2/prices/{ticker}-USD/historic?period=day";
        response = await client.GetAsync(historicPricesUrl);
        responseString = await response.Content.ReadAsStringAsync();
        jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
        var historicPrices = jsonResponse.data.prices;
        if (historicPrices.Count >= 2)
        {
            previousClose = (double)historicPrices[1].price;
        }
        double percentChange = (currentPrice - previousClose) / previousClose * 100;
        return Tuple.Create(currentPrice, percentChange);
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