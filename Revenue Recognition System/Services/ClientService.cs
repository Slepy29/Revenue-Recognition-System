using Microsoft.EntityFrameworkCore;
using Revenue_Recognition_System.Data;
using Revenue_Recognition_System.DTOs;
using Revenue_Recognition_System.Models;

namespace Revenue_Recognition_System.Services;

public class ClientService : IClientService
{
    private readonly DatabaseContext _context;
    public ClientService(DatabaseContext context)
    {
        _context = context;
    }
    
    public async Task<Client?> GetClient(int clientId)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(e => e.ClientId == clientId);
    }

    public async Task<Client> AddClient(AddClientDTO newClient)
    {
        Client client = new Client()
        {
            Email = newClient.Email,
            PhoneNumber = newClient.PhoneNumber,
            KRS = newClient.KRS,
            PESEL = newClient.PESEL,
            IsCompany = newClient.IsCompany,
            Name = newClient.Name
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> DeleteClient(int clientId)
    {
        var client = await GetClient(clientId);
        if (client == null)
        {
            return false;
        }

        if (!client.IsCompany)
        {
            client.Name = null;
            client.Email = null;
            client.PhoneNumber = null;
            client.PESEL = null;
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return true;
        }
        
        return false;
    }

    public async Task<Client> UpdateClient(UpdateClientDTO updatedClient)
    {
        var existingClient = await _context.Clients.FindAsync(updatedClient.ClientId);
        if (existingClient == null)
        {
            return null;
        }

        existingClient.Name = updatedClient.Name;
        existingClient.Email = updatedClient.Email;
        existingClient.PhoneNumber = updatedClient.PhoneNumber;

        if (!existingClient.IsCompany)
        {
            existingClient.PESEL = existingClient.PESEL;
        }
        else
        {
            existingClient.KRS = existingClient.KRS;
        }

        _context.Clients.Update(existingClient);
        await _context.SaveChangesAsync();
        return existingClient;
    }
}