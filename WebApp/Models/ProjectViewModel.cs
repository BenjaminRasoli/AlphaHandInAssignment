using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ProjectViewModel
{
    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "Project Name", Prompt = "Enter Project Name")]
    public string ProjectName { get; set; } = null!;

    [Required(ErrorMessage = "is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "Description", Prompt = "Enter Description")]
    public string Description { get; set; } = null!;


}
