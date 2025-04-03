namespace Business.Models;

public abstract class ServiceResult
{
    public bool Succeded { get; set; }
    public string? Error { get; set; }
    public int StatusCode { get; set; }
}

