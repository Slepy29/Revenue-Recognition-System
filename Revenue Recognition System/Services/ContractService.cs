﻿using Microsoft.EntityFrameworkCore;
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
    
    public async Task<Contract> CreateContract(int clientId, int softwareId, int additionalYears)
    {
        var client = _context.Clients
            .Include(c => c.Contracts)
            .FirstOrDefault(c => c.ClientId == clientId);
        if (client == null)
        {
            throw new InvalidOperationException("Client not found.");
        }
        
        if (client.Contracts != null && client.Contracts.Any(c => c.SoftwareId == softwareId && c.EndDate >= DateTime.UtcNow))
        {
            throw new InvalidOperationException("Client has an active contract for this product.");
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
            EndDate = DateTime.UtcNow.AddDays(30),
            IsSigned = false,
            Price = software.BasePrice + additionalYears * 1000,
        };
        
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
            contract.IsSigned = true;
            contract.StartDate = DateTime.UtcNow;
            contract.EndDate = DateTime.UtcNow.AddYears(1);
            await _context.SaveChangesAsync();
        }

        return payment;
    }
    
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
                _context.Payments.Remove(payment);
            }

            _context.Contracts.Remove(contract);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
