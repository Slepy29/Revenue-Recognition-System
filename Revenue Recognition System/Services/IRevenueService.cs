namespace Revenue_Recognition_System.Services;

public interface IRevenueService
{
    public Task<decimal> CalculateCurrentRevenue(string currencyCode = "PLN", int? productId = null);
    
    public Task<decimal> CalculateExpectedRevenue(string currencyCode = "PLN", int? productId = null);
}