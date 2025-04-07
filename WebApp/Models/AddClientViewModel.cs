using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class AddClientViewModel
{

    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "Client Name", Prompt = "Input Client name")]
    public string ClientName { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Client Email", Prompt = "Input Client Email")]
    public string? ClientEmail { get; set; }

    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Client PhoneNumber", Prompt = "Input Client Phone Number")]
    public string? ClientPhoneNumber { get; set; }


}
