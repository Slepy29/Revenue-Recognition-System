using Microsoft.AspNetCore.Mvc;
using Revenue_Recognition_System.Services;

namespace Revenue_Recognition_System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractController : ControllerBase
{
    private readonly ContractService _contractService;

    public ContractController(ContractService contractService)
    {
        _contractService = contractService;
    }

    // Create Contract
    [HttpPost("create-contract")]
    public async Task<IActionResult> CreateContract(int clientId, int softwareId, int additionalYears)
    {
        try
        {
            var contract = await _contractService.CreateContract(clientId, softwareId, additionalYears);
            return Ok(contract);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Issue Payment for Contract
    [HttpPost("issue-payment")]
    public async Task<IActionResult> IssuePayment(int contractId, decimal amount)
    {
        try
        {
            var payment = await _contractService.IssuePayment(contractId, amount);
            return Ok(payment);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Check and Refund Expired Contracts (this could be called by a scheduled job)
    [HttpPost("check-expired-contracts")]
    public async Task<IActionResult> CheckAndRefundExpiredContracts()
    {
        try
        {
            var result = await _contractService.CheckAndRefundExpiredContracts();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
