using Business.Models;
using Data.Contexts;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Business.Services;

public interface IUserService
{
    Task<UserResult> AddUserToRole(string userId, string roleName);
    Task<UserResult> CreateUserAsync(SignUpFormData formData, string roleName = "User");
    Task<string> GetDisplayName(string userId);
    Task<UserResult> GetUserByIdAsync(string id);

    Task<UserResult<IEnumerable<User>>> GetUsersAsync();

}

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public async Task<UserResult<IEnumerable<User>>> GetUsersAsync()
    {
        var repositoryResult = await _userRepository.GetAllAsync<UserEntity>
            (
                orderByDescending: false,
                sortByColumn: x => x.FirstName!
            );

        var entities = repositoryResult.Result;
        var users = entities?.Select(entity => entity.MapTo<User>()) ?? [];


        return new UserResult<IEnumerable<User>>
        {
            Succeded = true,
            StatusCode = 200,
            Result = users
        };
    }

    public async Task<UserResult> AddUserToRole(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return new UserResult
            {
                Succeded = false,
                StatusCode = 404,
                Error = "Role not found"
            };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new UserResult
            {
                Succeded = false,
                StatusCode = 404,
                Error = "User not found"
            };
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return new UserResult
            {
                Succeded = true,
                StatusCode = 200
            };
        }
        else
        {
            return new UserResult
            {
                Succeded = false,
                StatusCode = 500,
                Error = "Unable to add user to role"
            };
        }
    }

    public async Task<UserResult> GetUserByIdAsync(string id)
    {
        var repositoryResult = await _userRepository.GetAsync(x => x.Id == id);

        var entity = repositoryResult.Result;
        if (entity == null)
            return new UserResult
            {
                Succeded = false,
                StatusCode = 404,
                Error = "User not found"
            };

        var user = entity.MapTo<User>();
        return new UserResult
        {
            Succeded = true,
            StatusCode = 200,
            User = user
        };
    }

    public async Task<UserResult> CreateUserAsync(SignUpFormData formData, string roleName = "User")
    {
        if (formData == null)
        {
            return new UserResult
            {
                Succeded = false,
                StatusCode = 400,
                Error = "form data cannot be null"
            };
        }

        var existsResult = await _userRepository.ExistsAsync(x => x.Email == formData.Email);
        if (existsResult.Succeded)
        {
            return new UserResult
            {
                Succeded = false,
                StatusCode = 409,
                Error = "user with same email arleady exists"
            };
        }

        try
        {
            var userEntity = formData.MapTo<UserEntity>();
            userEntity.UserName = formData.FirstName + formData.LastName;

            var result = await _userManager.CreateAsync(userEntity, formData.Password);

            if (result.Succeeded)
            {
                var users = await _userRepository.GetAllAsync<UserEntity>();
                if (users?.Result?.Count() == 1)
                {
                    var addToRoleResult = await AddUserToRole(userEntity.Id, "Admin");
                }
                else
                {
                    var addToRoleResult = await AddUserToRole(userEntity.Id, roleName);
                }

                if (result.Succeeded)
                {
                    return new UserResult
                    {
                        Succeded = true,
                        StatusCode = 201,
                    };
                }
                else
                {
                    return new UserResult
                    {
                        Succeded = false,
                        StatusCode = 201,
                        Error = "User created but not added to role",
                    };
                }
            }

            return new UserResult
            {
                Succeded = false,
                StatusCode = 500,
                Error = "Unable to create user"
            };


        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new UserResult
            {
                Succeded = false,
                StatusCode = 500,
                Error = ex.Message
            };
        }
    }
    public async Task<string> GetDisplayName(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return "";

        var user = await _userManager.FindByIdAsync(userId);
        return user == null ? "" : $"{user.FirstName} {user.LastName}";
    }
}
