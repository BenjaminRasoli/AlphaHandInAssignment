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

public class ProjectService(IProjectRepository projectRepository, IStatusService statusService) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IStatusService _statusService = statusService;

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
        var status = statusResult.Result;

        projectEntity.StatusId = status!.Id;

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
        var response = await _projectRepository.GetAllAsync
            (orderByDescending: true, sortBy: s => s.Created, where: null,
                include => include.User,
                include => include.Status,
                include => include.Client
            );

        return new ProjectResult<IEnumerable<Project>>
        {
            Succeded = true,
            StatusCode = 200,
            Result = response.Result
        };
    }

    public async Task<ProjectResult<Project>> GetProjectAsync(string id)
    {
        var response = await _projectRepository.GetAsync
            (
                where: x => x.Id == id,
                include => include.User,
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
