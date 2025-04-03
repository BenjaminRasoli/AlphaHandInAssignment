using Business.Services;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class ProjectsController(IProjectService projectService) : Controller
{
    private readonly IProjectService _projectService = projectService;

    //public async Task<IActionResult> Index()
    //{
    //    var model = new ProjectViewModel
    //    {
    //        Project = await _projectService.GetProjectsAsync()
    //    };
    //    return View(model);
    //}

    //[HttpPost]
    //public IActionResult Add(AddProjectViewModel model)
    //{
    //    return View();
    //}

    //[HttpPut]
    //public IActionResult Update(UpdateProjectVideModel model)
    //{
    //    return View();
    //}

    //[HttpDelete]
    //public IActionResult Delete(string id)
    //{
    //    return View();
    //}
}
