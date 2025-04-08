using Business.Models;
using Data.Entities;
using Data.Models;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IStatusService
{
    Task<StatusResult<Status>> GetStatusByIdAsync(int id);
    Task<StatusResult<Status>> GetStatusByNameAsync(string statusName);
    Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync();
}

public class StatusService(IStatusRepository statusRepository) : IStatusService
{
    private readonly IStatusRepository _statusRepository = statusRepository;

    public async Task<StatusResult<IEnumerable<Status>>> GetStatusesAsync()
    {
        var result = await _statusRepository.GetAllAsync(
    selector: entity => new Status
    {
        Id = entity.Id.ToString(),
        StatusName = entity.StatusName
    },
    orderByDescending: false,
    sortByColumn: x => x.Id
);


        var entities = result.Result;

        var statuses = entities?.Select(entity => new Status
        {
            Id = entity.Id.ToString(), 
            StatusName = entity.StatusName
        }) ?? [];

        if (result.Succeded)
        {
            return new StatusResult<IEnumerable<Status>>
            {
                Succeded = true,
                StatusCode = 200,
                Result = statuses
            };
        }
        else
        {
            return new StatusResult<IEnumerable<Status>>
            {
                Succeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error
            };
        }
    }

    public async Task<StatusResult<Status>> GetStatusByNameAsync(string statusName)
    {
        var result = await _statusRepository.GetAsync(x => x.StatusName == statusName);
        if (result.Succeded)
        {
            return new StatusResult<Status>
            {
                Succeded = true,
                StatusCode = 200,
                Result = result.Result
            };
        }
        else
        {
            return new StatusResult<Status>
            {
                Succeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error
            };
        }
    }

    public async Task<StatusResult<Status>> GetStatusByIdAsync(int id)
    {
        var result = await _statusRepository.GetAsync(x => x.Id == id);
        var entity = result.Result;

        if (entity == null)
        {
            return new StatusResult<Status>
            {
                Succeded = false,
                StatusCode = 404,
                Error = "Status not found"
            };
        }

        var status = entity.MapTo<Status>();

        if (result.Succeded)
        {
            return new StatusResult<Status>
            {
                Succeded = true,
                StatusCode = 200,
                Result = status
            };
        }
        else
        {
            return new StatusResult<Status>
            {
                Succeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error
            };
        }
    }

}
