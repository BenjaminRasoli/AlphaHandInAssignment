using Business.Models;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(SignInFormData formData);
    Task<AuthResult> SignOutAsync();
    Task<AuthResult> SignUpAsync(SignUpFormData formData);
}

public class AuthService(IUserService userService, SignInManager<UserEntity> signInManager) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;

    public async Task<AuthResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
        {
            return new AuthResult
            {
                Succeded = false,
                StatusCode = 400,
                Error = "Form data is null"
            };
        }

        var result = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, formData.IsPersistent, false);

        if (result.Succeeded)
        {
            return new AuthResult
            {
                Succeded = true,
                StatusCode = 201,
            };
        }
        else
        {
            return new AuthResult
            {
                Succeded = false,
                StatusCode = 401,
                Error = "Invlaid email or paswrod"
            };
        }
    }

    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
        {
            return new AuthResult
            {
                Succeded = false,
                StatusCode = 400,
                Error = "Form data is null"
            };
        }

        var result = await _userService.CreateUserAsync(formData);

        if (result.Succeded)
        {
             await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, false, false);

            return new AuthResult
            {
                Succeded = true,
                StatusCode = 201,
            };
        }
        else
        {
            return new AuthResult
            {
                Succeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error
            };
        }
    }

    public async Task<AuthResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthResult
        {
            Succeded = true,
            StatusCode = 200,
        };
    }


}
