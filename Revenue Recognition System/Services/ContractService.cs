using Revenue_Recognition_System.Data;
using Revenue_Recognition_System.Models;

namespace Revenue_Recognition_System.Services;

public class ContractService : IContractService
{
    private readonly DatabaseContext _context;

    public ContractService(DatabaseContext context)
    {
        _context = context;
    }

    // Create Contract
    public async Task<Contract> CreateContract(int clientId, int softwareId, int additionalYears)
    {
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null)
        {
            throw new InvalidOperationException("Client not found.");
        }

        if (client.Contracts.Any(c => c.SoftwareId == softwareId && c.EndDate >= DateTime.UtcNow))
        {
            throw new InvalidOperationException("Client has an active subscription or contract for this product.");
        }

        var software = await _context.Softwares.FindAsync(softwareId);
        if (software == null)
        {
            throw new InvalidOperationException("Software not found.");
        }

        var contract = new Contract
        {
            ClientId = clientId,
            SoftwareId = softwareId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30), // Initial period of 30 days for payment
            IsSigned = false, // Initial state before payment is completed
            Price = software.BasePrice + additionalYears * 1000, // Initial price calculation
        };

        // Apply discounts
        var applicableDiscounts = _context.Discounts
            .Where(d => d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow)
            .ToList();

        var highestDiscount = applicableDiscounts.Any() ? applicableDiscounts.Max(d => d.Percentage) : 0;
        var returningClientDiscount = client.Contracts.Any() ? 5 : 0;
        var finalDiscount = Math.Max(highestDiscount, returningClientDiscount);

        contract.Price -= contract.Price * (decimal)(finalDiscount / 100);

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        return contract;
    }

    // Issue Payment for Contract
    public async Task<Payment> IssuePayment(int contractId, decimal amount)
    {
        var contract = await _context.Contracts.FindAsync(contractId);
        if (contract == null || contract.EndDate < DateTime.UtcNow || contract.IsSigned)
        {
            throw new InvalidOperationException("Contract is either invalid, expired, or already signed.");
        }

        var payment = new Payment
        {
            ContractId = contractId,
            ClientId = contract.ClientId,
            Amount = amount,
            PaymentDate = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        var totalPayments = _context.Payments.Where(p => p.ContractId == contractId).Sum(p => p.Amount);
        if (totalPayments >= contract.Price)
        {
            contract.IsSigned = true; // Mark the contract as signed
            contract.StartDate = DateTime.UtcNow;
            contract.EndDate = DateTime.UtcNow.AddYears(1); // Set the actual contract duration after full payment
            await _context.SaveChangesAsync();
        }

        return payment;
    }

    // Refund payments if the contract is not paid within the timeframe
    public async Task<bool> CheckAndRefundExpiredContracts()
    {
        var expiredContracts = _context.Contracts
            .Where(c => !c.IsSigned && c.EndDate < DateTime.UtcNow)
            .ToList();

        foreach (var contract in expiredContracts)
        {
            var payments = _context.Payments.Where(p => p.ContractId == contract.ContractId).ToList();
            foreach (var payment in payments)
            {
                // Implement refund logic here, e.g., call payment gateway API to process refunds
                _context.Payments.Remove(payment);
            }

            _context.Contracts.Remove(contract);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
