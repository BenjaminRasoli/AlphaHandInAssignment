using Business.Models;
using Business.Services;
using Data.Entities;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace Presentation.Controllers;

[Authorize(Roles = "Admin")]
public class ClientsController(IClientService clientService, INotificationService notificationService) : Controller
{

    private readonly IClientService _clientService = clientService;
    private readonly INotificationService _notificationService = notificationService;

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

        var formData = new AddClientFormData
        {
            ClientName = model.ClientName,
            Email = model.ClientEmail,
            PhoneNumber = model.ClientPhoneNumber
        };

        var result = await _clientService.CreateClientAsync(formData);

        if (result != null)
        {
            var notificationFormData = new NotificationFormData
            {
                NotificationTypeId = 1,
                NotificationTargetId = 1,
                Message = $"{formData.ClientName} client created.",
            };

            await _notificationService.AddNotificationAsync(notificationFormData);
        }

        if (result!.Succeded)
        {
            return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = result.Error;
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

}
