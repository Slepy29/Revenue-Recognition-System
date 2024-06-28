using Revenue_Recognition_System.Models;

namespace Revenue_Recognition_System.Services;

public interface IContractService
{
    public Task<Contract> CreateContract(int clientId, int softwareId, int additionalYears);
    
    public Task<Payment> IssuePayment(int contractId, decimal amount);
    
    public Task<bool> CheckAndRefundExpiredContracts();
}