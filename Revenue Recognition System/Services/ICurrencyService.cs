namespace Revenue_Recognition_System.Services;

public interface ICurrencyService
{
    Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency);
}