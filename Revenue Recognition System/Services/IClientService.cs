using Revenue_Recognition_System.Models;

namespace Revenue_Recognition_System.Services;

public interface IClientService
{
    Task<Client> GetClient(int clientId);
    Task<Client> AddClient(Client client);
    Task<bool> DeleteClient(int clientId);
    Task<Client> UpdateClient(Client updatedClient);
}
