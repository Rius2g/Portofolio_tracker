using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Web;


namespace API 
{
   public class Get
    {
        static public string API_KEY = "E6TUK887BIOZOONH";
        public async Task<Tuple<string, double, double>> GetStockPriceAndPercentChange(string ticker)
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

        public async Task<Tuple<string, double, double, string>> GetStockPriceAndPercentChangeByISIN(string isin)
        {
            var apiUrl = $"https://www.morningstar.no/no/funds/snapshot/snapshot.aspx?id={isin}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(apiUrl);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Extract name, price, previous close, and security type values from the HTML response
            var nameStartTag = "<h1 class=\"heading1\">";
            var nameEndTag = "</h1>";
            var nameStartIndex = responseContent.IndexOf(nameStartTag) + nameStartTag.Length;
            var nameEndIndex = responseContent.IndexOf(nameEndTag, nameStartIndex);
            var name = responseContent.Substring(nameStartIndex, nameEndIndex - nameStartIndex);

            var priceStartTag = "<span id=\"MainContent_fundOverview_lblPrice\">";
            var priceEndTag = "</span>";
            var priceStartIndex = responseContent.IndexOf(priceStartTag) + priceStartTag.Length;
            var priceEndIndex = responseContent.IndexOf(priceEndTag, priceStartIndex);
            var priceString = responseContent.Substring(priceStartIndex, priceEndIndex - priceStartIndex);
            var price = double.Parse(priceString, CultureInfo.InvariantCulture);

            var prevCloseStartTag = "<span id=\"MainContent_fundOverview_lblPreviousClose\">";
            var prevCloseEndTag = "</span>";
            var prevCloseStartIndex = responseContent.IndexOf(prevCloseStartTag) + prevCloseStartTag.Length;
            var prevCloseEndIndex = responseContent.IndexOf(prevCloseEndTag, prevCloseStartIndex);
            var prevCloseString = responseContent.Substring(prevCloseStartIndex, prevCloseEndIndex - prevCloseStartIndex);
            var previousClose = double.Parse(prevCloseString, CultureInfo.InvariantCulture);

            var typeStartTag = "<td class=\"line heading leftColumn\">";
            var typeEndTag = "</td>";
            var typeStartIndex = responseContent.IndexOf(typeStartTag) + typeStartTag.Length;
            var typeEndIndex = responseContent.IndexOf(typeEndTag, typeStartIndex);
            var typeString = responseContent.Substring(typeStartIndex, typeEndIndex - typeStartIndex);
            var type = HttpUtility.HtmlDecode(typeString).Trim();

            var percentChange = (price - previousClose) / previousClose * 100;
            return Tuple.Create(name, price, percentChange, type);
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
        
    }

}