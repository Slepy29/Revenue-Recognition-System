using Microsoft.EntityFrameworkCore;
using Revenue_Recognition_System.Data;

namespace Revenue_Recognition_System.Services;

public class RevenueService : IRevenueService
{
    private readonly DatabaseContext _context;
    private readonly ICurrencyService _currencyService;

    public RevenueService(DatabaseContext context, ICurrencyService currencyService)
    {
        _context = context;
        _currencyService = currencyService;
    }
    
    // Calculate Current Revenue
    public async Task<decimal> CalculateCurrentRevenue(string currencyCode = "PLN", int? productId = null)
    {
        var payments = _context.Payments.AsQueryable();

        if (productId.HasValue)
        {
            payments = payments.Where(p => p.Contract.SoftwareId == productId);
        }

        var totalRevenue = await payments.SumAsync(p => p.Amount);

        if (currencyCode != "PLN")
        {
            var exchangeRate = await _currencyService.GetExchangeRateAsync("PLN", currencyCode);
            totalRevenue *= exchangeRate;
        }

        return totalRevenue;
    }

    // Calculate Expected Revenue
    public async Task<decimal> CalculateExpectedRevenue(string currencyCode = "PLN", int? productId = null)
    {
        var signedContracts = _context.Contracts.AsQueryable().Where(c => c.IsSigned);
        var unsignedContracts = _context.Contracts.AsQueryable().Where(c => !c.IsSigned && c.EndDate >= DateTime.UtcNow);

        if (productId.HasValue)
        {
            signedContracts = signedContracts.Where(c => c.SoftwareId == productId);
            unsignedContracts = unsignedContracts.Where(c => c.SoftwareId == productId);
        }

        var signedRevenue = await signedContracts.SumAsync(c => c.Price);
        var unsignedRevenue = await unsignedContracts.SumAsync(c => c.Price);

        var totalExpectedRevenue = signedRevenue + unsignedRevenue;

        if (currencyCode != "PLN")
        {
            var exchangeRate = await _currencyService.GetExchangeRateAsync("PLN", currencyCode);
            totalExpectedRevenue *= exchangeRate;
        }

        return totalExpectedRevenue;
    }
}