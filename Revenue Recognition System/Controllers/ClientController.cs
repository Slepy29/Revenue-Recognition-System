using Microsoft.AspNetCore.Mvc;
using Revenue_Recognition_System.Models;
using Revenue_Recognition_System.Services;

namespace Revenue_Recognition_System.Controllers;

public class ClientController : ControllerBase
{
    private readonly ClientService _clientService;

    public ClientController(ClientService clientService)
    {
        _clientService = clientService;
    }
    
    [HttpPost("add-client")]
    public async Task<IActionResult> AddClient(Client client)
    {
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
    public async Task<IActionResult> UpdateClient(Client client)
    {
        var updatedClient = await _clientService.UpdateClient(client);
        if (updatedClient == null)
        {
            return NotFound();
        }
        return Ok(updatedClient);
    }
}
