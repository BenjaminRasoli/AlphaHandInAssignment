using Azure;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData);
    Task<ProjectResult> DeleteProjectAsync(string id);
    Task<ProjectResult<Project>> GetProjectAsync(string id);
    Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync();
}

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService, IClientService clientService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;
    private readonly IClientService _clientService = clientService;

    public async Task<ProjectResult> CreateProjectAsync(AddProjectFormData formData)
    {
        if (formData == null)
        {
            return new ProjectResult
            {
                Succeded = false,
                StatusCode = 400,
                Error = "Not all required fields are filled"
            };
        }

        var projectEntity = formData.MapTo<ProjectEntity>();

        var statusResult = await _statusService.GetStatusByIdAsync(1);
        projectEntity.StatusId = 1;

        var clientResult = await _clientService.GetClientByIdAsync(formData.ClientId);
        projectEntity.ClientId = formData.ClientId;


        var result = await _projectRepository.AddAsync(projectEntity);

        if (result.Succeded)
        {
            return new ProjectResult
            {
                Succeded = true,
                StatusCode = 201,
            };
        }
        else
        {
            return new ProjectResult
            {
                Succeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error
            };
        }
    }

    public async Task<ProjectResult<IEnumerable<Project>>> GetProjectsAsync()
    {
        var result = await _projectRepository.GetAllAsync(
            orderByDescending: true,
            sortByColumn: s => s.Created,
            filterBy: null,
            sortBy: null,
            where: null,
            take: 0,
            //include => include.User,
            include => include.Status,
            include => include.Client
        );

        if (result.Succeded)
        {
            var projects = result.Result!.Select(x => x.MapTo<Project>());

            return new ProjectResult<IEnumerable<Project>>
            {
                Succeded = true,
                StatusCode = 200,
                Result = projects
            };
        }
        return new ProjectResult<IEnumerable<Project>>
        {
            Succeded = false,
            StatusCode = 500
        };

    }


    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync
            (
                where: x => x.Id == id,
                //include => include.User,
                include => include.Status,
                include => include.Client
            );

        if (response.Succeded)
        {
            return new ProjectResult<Project>
            {
                Succeded = true,
                StatusCode = 200,
                Result = response.Result
            };
        }
        else
        {
            return new ProjectResult<Project>
            {
                Succeded = false,
                StatusCode = 200,
                Error = "Project not found"
            };
        }
    }

    public async Task<ProjectResult> DeleteProjectAsync(string id)
    {
        var projectEntity = await _projectRepository.GetAsync(x => x.Id == id);

        if (projectEntity.Succeded)
        {
            //var result = await _projectRepository.DeleteAsync();

            return new ProjectResult
            {
                Succeded = true,
                StatusCode = 200
            };
        }
        else
        {
            return new ProjectResult
            {
                Succeded = false,
                StatusCode = 404,
                Error = "Project not found"
            };
        }
    }

}
