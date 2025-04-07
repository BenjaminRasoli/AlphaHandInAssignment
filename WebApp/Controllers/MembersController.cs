using Business.Models;
using Business.Services;
using Data.Entities;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{

    [Authorize(Roles = "Admin")]
    public class MembersController(IAuthService authService, IUserService userService, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager) : Controller
    {

        private readonly IAuthService _authService = authService;
        private readonly IUserService _userService = userService;
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
      

        [Route("admin/members")]

        public async Task<IActionResult> Index()
        {
            var roles = await GetRolesSelectListAsync();
            
            var vm = new MembersViewModel
            {
                AddMemberviewModel = new AddMemberviewModel
                {
                    Roles = roles
                },
            };
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(AddMemberviewModel model, string returnUrl = "~/")
        {
            ViewBag.ErrorMessage = null;
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var signUpFormData = model.MapTo<SignUpFormData>();
            signUpFormData.Password = "BytMig123!";
            var selectedRole = model.RoleId ?? "User"; 

            var result = await _authService.SignUpAsync(signUpFormData, selectedRole);

            if (result.Succeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Failed to create user.");
            return View(model);
        }



        private async Task<IEnumerable<SelectListItem>> GetRolesSelectListAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var rolesList = roles.Select(role => new SelectListItem
            {
                Value = role.Name,  
                Text = role.Name
            });

            return rolesList;
        }


    }
}
