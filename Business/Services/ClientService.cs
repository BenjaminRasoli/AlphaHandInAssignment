using Business.Models;
using Data.Repositories;
using Domain.Extensions;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult> GetClientsAsync();
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        return result.MapTo<ClientResult>();
    }
}
