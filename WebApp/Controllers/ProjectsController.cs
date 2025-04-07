﻿using Business.Models;
using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize]
public class ProjectsController(IStatusService statusService, IClientService clientService, IProjectService projectService) : Controller
{
    private readonly IStatusService _statusService = statusService;
    private readonly IClientService _clientService = clientService;
    private readonly IProjectService _projectService = projectService;

    #region List

    [Route("admin/projects")]
    public async Task<IActionResult> Index()
    {
        var clients = await GetClientsSelectListAsync();
        var statuses = await GetStatusesSelectListAsync();
        var projects = await GetProjectsAsync();

        var vm = new ProjectsViewModel
        {
            Projects = projects,
            AddProjectViewModel = new AddProjectViewModel()
            {
                Clients = clients,
            },
            EditProjectViewModel = new EditProjectViewModel()
            {
                Clients = clients,
                Statuses = statuses,
            }
        };

        return View(vm);
    }

    #endregion

    #region Add

    //[HttpGet]
    //public async Task<IActionResult> Add()
    //{
    //    var vm = new AddProjectViewModel
    //    {
    //        Clients = await GetClientsSelectListAsync(),
    //    };
    //    return PartialView("~/Views/Shared/Partials/Project/_AddProjectModal.cshtml", vm);
    //}

    [HttpPost]
    public async Task<IActionResult> Add(AddProjectViewModel model)
    {
        if (ModelState.IsValid)
        {
            var formData = new AddProjectFormData
            {
                ClientId = model.ClientId,
                ProjectName = model.ProjectName,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Budget = model.Budget,
                Image = model.Image
            };

            var result = await _projectService.CreateProjectAsync(formData);

            if (result.Succeded)
            {
                TempData["SuccessMessage"] = "Project created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", result.Error ?? "An error occurred while creating the project.");
            }
        }

        model.Clients = await GetClientsSelectListAsync();
        return PartialView("~/Views/Shared/Partials/Project/_AddProjectModal.cshtml", model);
    }



    #endregion

    #region Edit

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var projectResult = await _projectService.GetProjectAsync(id);
        if (!projectResult.Succeded || projectResult.Result == null)
            return NotFound();

        var project = projectResult.Result;


        var vm = new EditProjectViewModel
        {
            Id = project.Id,
            ImageUrl = project.Image,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            ClientId = project.Client.Id,
            StatusId = project.Status.Id,

            Clients = await GetClientsSelectListAsync(),
            Statuses = await GetStatusesSelectListAsync(),
        };

        return PartialView("~/Views/Shared/Partials/Project/_EditProjectModal.cshtml", vm);
    }

    #endregion

    #region Helpers

    private async Task<IEnumerable<SelectListItem>> GetClientsSelectListAsync()
    {
        var result = await _clientService.GetClientsAsync();
        var ClientList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id,
            Text = s.ClientName,
        });

        return ClientList!;
    }

    private async Task<IEnumerable<SelectListItem>> GetStatusesSelectListAsync()
    {
        var result = await _statusService.GetStatusesAsync();
        var statusList = result.Result?.Select(s => new SelectListItem
        {
            Value = s.Id,
            Text = s.StatusName
        });

        return statusList!;
    }


    private async Task<IEnumerable<Project>> GetProjectsAsync()
    {
        IEnumerable<Project> projects = [];
        try
        {
            var projectResult = await _projectService.GetProjectsAsync();
            if (projectResult.Succeded && projectResult.Result != null)
            {
                projects = projectResult.Result;
            }
        }
        catch (Exception ex)
        {
            projects = [];
        }
        return projects;

    }

    #endregion
}