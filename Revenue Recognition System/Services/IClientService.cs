using Revenue_Recognition_System.DTOs;
using Revenue_Recognition_System.Models;

namespace Revenue_Recognition_System.Services;

public interface IClientService
{
    Task<Client> GetClient(int clientId);
    Task<Client> AddClient(AddClientDTO newClient);
    Task<bool> DeleteClient(int clientId);
    Task<Client> UpdateClient(UpdateClientDTO updatedClient);
}
