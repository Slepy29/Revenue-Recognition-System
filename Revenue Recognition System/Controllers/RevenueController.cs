using Microsoft.AspNetCore.Mvc;
using Revenue_Recognition_System.Services;

namespace Revenue_Recognition_System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }
    
    [HttpGet("current-revenue")]
    public async Task<IActionResult> GetCurrentRevenue([FromQuery] string currencyCode = "PLN", [FromQuery] int? productId = null)
    {
        try
        {
            var revenue = await _revenueService.CalculateCurrentRevenue(currencyCode, productId);
            return Ok(revenue);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("expected-revenue")]
    public async Task<IActionResult> GetExpectedRevenue([FromQuery] string currencyCode = "PLN", [FromQuery] int? productId = null)
    {
        try
        {
            var revenue = await _revenueService.CalculateExpectedRevenue(currencyCode, productId);
            return Ok(revenue);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}