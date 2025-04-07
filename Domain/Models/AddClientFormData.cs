namespace Domain.Models;

public class AddClientFormData
{
    public string ClientName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
