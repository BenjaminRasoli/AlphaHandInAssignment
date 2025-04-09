using Business.Services;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IAuthService authService, IUserService userService, INotificationService notificationService) : Controller
{
    private readonly IAuthService _authService = authService;
    private readonly IUserService _userService = userService;
    private readonly INotificationService _notificationService = notificationService;


    public IActionResult SignUp(string returnUrl = "~/")
    {
        ViewBag.ReturnUrl = returnUrl;

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model, string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = null;
        ViewBag.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Image = "user-template.svg";
        var signUpFormData = model.MapTo<SignUpFormData>();
        var role = "User";
        var result = await _authService.SignUpAsync(signUpFormData, role);

        if (result.Succeded)
        {
            return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = result.Error;
        return View(model);
    }


    public IActionResult SignIn()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl = "~/")
    {
        ViewBag.ErrorMessage = null;
        ViewBag.ReturnUrl = returnUrl;


        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var signInFormData = model.MapTo<SignInFormData>();

        var result = await _authService.SignInAsync(signInFormData);

        if (result.Succeded)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userResult = await _userService.GetUserByIdAsync(userId!);
            var user = userResult.User;

            if (user != null)
            {
                var notificationFormData = new NotificationFormData
                {
                    NotificationTypeId = 1,
                    NotificationTargetId = 1,
                    Message = $"{user.FirstName} {user.LastName} signed in.",
                    Image = user.Image
                };

                await _notificationService.AddNotificationAsync(notificationFormData);
            }
            return LocalRedirect(returnUrl);
        }

        ViewBag.ErrorMessage = result.Error;
        return View(model);
    }

    [Route("auth/logout")]
    public async Task<IActionResult> Logout()
    {
        await _authService.SignOutAsync();
        return LocalRedirect("~/");
    }
}
