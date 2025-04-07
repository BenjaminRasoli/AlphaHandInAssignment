using Domain.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class AddProjectFormData
{
    public IFormFile? Image { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string ClientId { get; set; } = null!;
}
