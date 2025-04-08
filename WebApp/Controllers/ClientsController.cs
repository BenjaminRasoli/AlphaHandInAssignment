using Business.Services;
using Data.Entities;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace Presentation.Controllers;

[Authorize(Roles = "Admin")]
public class ClientsController(IClientService clientService, INotificationService notificationService, IWebHostEnvironment env) : Controller
{

    private readonly IClientService _clientService = clientService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IWebHostEnvironment _env = env;


    [Route("admin/clients")]
    public async Task<IActionResult> Index()
    {
        var clientResult = await _clientService.GetClientsAsync();

        if (clientResult.Succeded)
        {
            var vm = new ClientsViewModel
            {
                Clients = clientResult.Result!,
                AddClientViewModel = new AddClientViewModel()
            };
            return View(vm);
        }

        return View(new ClientsViewModel
        {
            Clients = new List<Client>(),
            AddClientViewModel = new AddClientViewModel()
        });
    }


    [HttpPost]
    public async Task<IActionResult> Add(AddClientViewModel model, string returnUrl = "~/")
    {
        ViewBag.Message = null;
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string? fileName = null;

        if (model.ProfileImage != null && model.ProfileImage.Length > 0)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadFolder);

            fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProfileImage.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await model.ProfileImage.CopyToAsync(stream);
        }

        var formData = new AddClientFormData
        {
            ClientName = model.ClientName,
            Email = model.ClientEmail,
            PhoneNumber = model.ClientPhoneNumber,
            ProfileImage = fileName 
        };

        var result = await _clientService.CreateClientAsync(formData);

        if (result != null && result.Succeded)
        {
            var notificationFormData = new NotificationFormData
            {
                NotificationTypeId = 1,
                NotificationTargetId = 1,
                Message = $"{formData.ClientName} client created.",
                Image = fileName
            };

            await _notificationService.AddNotificationAsync(notificationFormData);
            return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = result?.Error ?? "Something went wrong.";
        return View(model);
    }



    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var clientResult = await _clientService.GetClientByIdAsync(id);
        if (clientResult.Succeded)
        {
            var client = clientResult.Result!.MapTo<ClientEntity>();

            var result = await _clientService.DeleteClientAsync(client);
            if (result.Succeded)
            {
                return Ok();
            }
        }
        return BadRequest();
    }

    public IActionResult Upload()
    {
        return View();
    }

    //[HttpPost]
    //public async Task<IActionResult> Upload(AddClientFormData model)
    //{
    //    if (model.ProfileImage == null)
    //    {
    //        return View(model);
    //    }

    //    var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
    //    Directory.CreateDirectory(uploadFolder);
    //    var filePath = Path.Combine(uploadFolder, Path.GetFileName(model.ProfileImage.FileName));

    //    using (var stream = new FileStream(filePath, FileMode.Create))
    //    {
    //        await model.ProfileImage.CopyToAsync(stream);
    //    }

    //    ViewBag.Message("file uplaoded");

    //        return View();
    //}

}
