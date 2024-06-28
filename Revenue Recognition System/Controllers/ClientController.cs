using Microsoft.AspNetCore.Mvc;
using Revenue_Recognition_System.DTOs;
using Revenue_Recognition_System.Services;

namespace Revenue_Recognition_System.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }
    
    [HttpPost("add-client")]
    public async Task<IActionResult> AddClient(AddClientDTO client)
    {
        if (client.IsCompany && String.IsNullOrEmpty(client.KRS))
            return BadRequest("No KRS");
        if (!client.IsCompany && String.IsNullOrEmpty(client.PESEL))
            return BadRequest("No PESEL");
        var addedClient = await _clientService.AddClient(client);
        return Ok(addedClient);
    }
    
    [HttpDelete("delete-client/{id}")]
    public async Task<IActionResult> SoftDeleteClient(int id)
    {
        var result = await _clientService.DeleteClient(id);
        if (!result)
        {
            return NotFound();
        }
        return Ok();
    }
    
    [HttpPut("update-client")]
    public async Task<IActionResult> UpdateClient(UpdateClientDTO client)
    {
        var updatedClient = await _clientService.UpdateClient(client);
        if (updatedClient == null)
        {
            return NotFound();
        }
        return Ok(updatedClient);
    }
}
