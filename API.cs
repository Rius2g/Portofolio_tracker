using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Web;


namespace API 
{
   public class Get
    {   
        public async Task<Tuple<string, double, double>> GetStockPriceAndPercentChange(string ticker, string API_KEY)
        {
            try
            {
            var apiUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&apikey={API_KEY}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JObject.Parse(responseContent)["Global Quote"];

            var stockName = responseData.Value<string>("01. symbol");
            var stockPrice = responseData.Value<double>("05. price");
            var previousClose = responseData.Value<double>("08. previous close");
            var percentChange = (stockPrice - previousClose) / previousClose * 100;

            return Tuple.Create(stockName, stockPrice, percentChange);
                
            }
            catch (System.Exception)
            {
                
                throw;
            }
            
        }

    public async Task<Tuple<string, double, double, string>> GetStockPriceAndPercentChangeByISIN(string isin, string API_KEY)
    {
        try
        {
        var apiUrl = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={isin}&apikey={API_KEY}";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(apiUrl);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Parse the JSON response to extract the name, price, and previous close values
        var json = JObject.Parse(responseContent);
        var name = (string)json["Global Quote"]["01. symbol"];
        var priceString = (string)json["Global Quote"]["05. price"];
        var price = double.Parse(priceString, CultureInfo.InvariantCulture);
        var previousCloseString = (string)json["Global Quote"]["08. previous close"];
        var previousClose = double.Parse(previousCloseString, CultureInfo.InvariantCulture);

        // Calculate the percent change
        var percentChange = (price - previousClose) / previousClose * 100;

        return Tuple.Create(name, price, percentChange, "Stock");
            
        }
        catch (System.Exception)
        {
            
            throw;
        }
    }


       public async Task<Tuple<string, double, double>> GetCryptoPriceAndPercentChange(string ticker)
       {
        using var httpClient = new HttpClient();
        string url = $"https://api.coinbase.com/v2/prices/{ticker}-USD/spot";
        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string responseString = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
            string cryptoName = jsonResponse.data.id;
            double currentPrice = jsonResponse.data.amount;
            double previousClose = jsonResponse.data.amount;
            var historicPricesUrl = $"https://api.coinbase.com/v2/prices/{ticker}-USD/historic?period=day";
            response = await httpClient.GetAsync(historicPricesUrl);
            responseString = await response.Content.ReadAsStringAsync();
            jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString);
            var historicPrices = jsonResponse.data.prices;
            if (historicPrices.Count >= 2)
            {
                previousClose = (double)historicPrices[1].price;
            }
            double percentChange = (currentPrice - previousClose) / previousClose * 100;
            return Tuple.Create(cryptoName, currentPrice, percentChange);
        }
        else
        {
            throw new Exception($"Failed to fetch crypto price for {ticker} with status code {response.StatusCode}.");
        }

        }

        public async Task<Tuple<string, double, double>> GetMutualFundPrice(string ticker)
        {
           HttpClient client = new HttpClient();

        string url = $"https://finance.yahoo.com/quote/{ticker}";
        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();

        // Find the name of the mutual fund
        int index = responseBody.IndexOf("data-reactid=\"1\">") + 19;
        string nameString = responseBody.Substring(index, responseBody.IndexOf("(", index) - index).Trim();
        string name = HttpUtility.HtmlDecode(nameString);

        // Find the price of the mutual fund
        index = responseBody.IndexOf("data-reactid=\"50\">") + 18;
        string priceString = responseBody.Substring(index, 10);
        priceString = priceString.Replace(",", "");
        if (double.TryParse(priceString, out double price))
        {
            // Find the change over 24h of the mutual fund
            index = responseBody.IndexOf("data-reactid=\"51\">") + 18;
            string changeString = responseBody.Substring(index, 10);
            changeString = changeString.Replace(",", "");
            if (double.TryParse(changeString, out double change))
            {
                return Tuple.Create(name, price, change);
            }
            else
            {
                throw new Exception("Failed to parse mutual fund change over 24h.");
            }
        }
        else
        {
            throw new Exception("Failed to parse mutual fund price.");
        }
        }

        public async Task<double> GetCurrencyExchangeRate(string fromCurrency, string toCurrency, string API_KEY)
        {
            try
            {
            //API_KEY = db.GetVantageKey();    
            var apiUrl = $"https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={fromCurrency}&to_currency={toCurrency}&apikey={API_KEY}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JObject.Parse(responseContent)["Realtime Currency Exchange Rate"];

            var exchangeRate = responseData.Value<double>("5. Exchange Rate");
            return exchangeRate;
            }
            catch (System.Exception)
            {
                
                throw;
            }

        }
    }

}