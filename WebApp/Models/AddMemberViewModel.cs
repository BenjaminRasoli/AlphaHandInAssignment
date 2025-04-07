using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class AddMemberviewModel
{
    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "First Name", Prompt = "Enter first name")]
    public string FirstName { get; set; } = null!;


    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name", Prompt = "Enter last name")]
    public string LastName { get; set; } = null!;


    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;

    //[Required(ErrorMessage = "is required.")]
    //[DataType(DataType.Password)]
    //[Display(Name = "Password", Prompt = "Enter password")]
    //public string Password { get; set; } = null!;


    //[Required(ErrorMessage = "is required")]
    //[Compare(nameof(Password), ErrorMessage = "password must be confirmed.")]
    //[DataType(DataType.Password)]
    //[Display(Name = "Confirm Password", Prompt = "Confirm password")]
    //public string ConfirmPassword { get; set; } = null!;



    [Required]
    [Display(Name = "Role", Prompt = "Select role")]
    public string RoleId { get; set; } = null!;

    public IEnumerable<SelectListItem> Roles { get; set; } = [];

}
