using Business.Services;
using Data.Entities;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Controllers
{

    [Authorize(Roles = "Admin")]
    public class MembersController(IAuthService authService, IUserService userService, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment env) : Controller
    {

        private readonly IAuthService _authService = authService;
        private readonly IUserService _userService = userService;
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IWebHostEnvironment _env = env;

        //public async Task<IActionResult> Index()
        //{
        //    var roles = await GetRolesSelectListAsync();
        //    var users = await _userService.GetUsersAsync();


        //    var vm = new MembersViewModel
        //    {
        //        Users = users,
        //        AddMemberviewModel = new AddMemberviewModel
        //        {
        //            Roles = roles
        //        },
        //    };
        //    return View(vm);
        //}

        [Route("admin/members")]
        public async Task<IActionResult> Index()
        {
            var roles = await GetRolesSelectListAsync();
            var userResult = await _userService.GetUsersAsync();

            if (!userResult.Succeded)
            {
                ViewBag.ErrorMessage = "Failed to retrieve users.";
                return View(new MembersViewModel());
            }

            var users = userResult.Result ?? [];

            var vm = new MembersViewModel
            {
                Users = users,
                AddMemberviewModel = new AddMemberviewModel
                {
                    Roles = roles
                },
            };

            return View(vm);
        }



        //[HttpPost]
        //public async Task<IActionResult> Create(AddMemberviewModel model, string returnUrl = "~/")
        //{
        //    ViewBag.ErrorMessage = null;
        //    ViewBag.ReturnUrl = returnUrl;

        //    if (!ModelState.IsValid)
        //    {
        //        return View("Index", model);
        //    }

        //    var signUpFormData = model.MapTo<SignUpFormData>();
        //    signUpFormData.Password = "BytMig123!";
        //    var selectedRole = model.RoleId ?? "User";

        //    var result = await _authService.SignUpAsync(signUpFormData, selectedRole);

        //    if (result.Succeded)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    ModelState.AddModelError("", "Failed to create user.");
        //    return View(model);
        //}

        [HttpPost]
        public async Task<IActionResult> Create(AddMemberviewModel model, string returnUrl = "~/")
        {
            ViewBag.ErrorMessage = null;
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            string? uploadedFileName = null;

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadFolder);

                uploadedFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadFolder, uploadedFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await model.ProfileImage.CopyToAsync(stream);
            }

            var signUpFormData = model.MapTo<SignUpFormData>();
            signUpFormData.Password = "BytMig123!";
            signUpFormData.Image = uploadedFileName;
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
