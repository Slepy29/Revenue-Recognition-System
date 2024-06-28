using Newtonsoft.Json;

namespace Revenue_Recognition_System.Services;

public class CurrencyService : ICurrencyService
{
    private readonly HttpClient _httpClient;

    public CurrencyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
    {
        var response = await _httpClient.GetStringAsync($"https://api.exchangerate-api.com/v4/latest/{fromCurrency}");
        var data = JsonConvert.DeserializeObject<ExchangeRateResponse>(response);
        return data.Rates[toCurrency];
    }

    private class ExchangeRateResponse
    {
        public Dictionary<string, decimal> Rates { get; set; }
    }
}