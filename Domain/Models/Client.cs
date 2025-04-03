using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class Client
{
    public int Id { get; set; }
    public string ClientName { get; set; } = null!;
}

