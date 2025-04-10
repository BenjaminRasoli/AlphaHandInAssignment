﻿using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IClientService
{
    Task<ClientResult<Client>> CreateClientAsync(AddClientFormData form);
    Task<ClientResult<IEnumerable<Client>>> DeleteClientAsync(ClientEntity entity);
    Task<ClientResult<Client>> GetClientByIdAsync(string id);
    Task<ClientResult<IEnumerable<Client>>> GetClientsAsync();
}

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    public async Task<ClientResult<IEnumerable<Client>>> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        if (result.Succeded)
        {
            var clients = result.Result!.Select(entity => entity.MapTo<Client>()).ToList();
            return new ClientResult<IEnumerable<Client>>
            {
                Succeded = result.Succeded,
                StatusCode = result.StatusCode,
                Result = clients
            };
        }

        return new ClientResult<IEnumerable<Client>>
        {
            Succeded = result.Succeded,
            StatusCode = result.StatusCode,
        };
    }

    public async Task<ClientResult<IEnumerable<Client>>> DeleteClientAsync(ClientEntity entity)
    {
        var result = await _clientRepository.DeleteAsync(entity);
        if (result.Succeded)
        {
            return new ClientResult<IEnumerable<Client>>
            {
                Succeded = result.Succeded,
                StatusCode = result.StatusCode,
            };
        }

        return new ClientResult<IEnumerable<Client>>
        {
            Succeded = result.Succeded,
            StatusCode = result.StatusCode,
            Result = []
        };
    }

    public async Task<ClientResult<Client>> GetClientByIdAsync(string id)
    {
        var result = await _clientRepository.GetAsync(x => x.Id == id);
        if (result.Succeded)
        {
            var client = result.Result!.MapTo<Client>();
            return new ClientResult<Client>
            {
                Succeded = true,
                StatusCode = 200,
                Result = client
            };
        }
        return new ClientResult<Client>
        {
            Succeded = false,
            StatusCode = result.StatusCode,
            Error = result.Error
        };
    }

    public async Task<ClientResult<Client>> CreateClientAsync(AddClientFormData form)
    {
        var result = await _clientRepository.AddAsync(new ClientEntity
        {
            ClientName = form.ClientName,
            Email = form.Email,
            PhoneNumber = form.PhoneNumber,
            ProfileImage = form.ProfileImage
            
        });
        if (result.Succeded)
        {
            var client = result.Result!.MapTo<Client>();
            return new ClientResult<Client>
            {
                Succeded = true,
                StatusCode = 200,
                Result = client
            };
        }
        return new ClientResult<Client>
        {
            Succeded = false,
            StatusCode = result.StatusCode,
            Error = result.Error
        };
    }


}
