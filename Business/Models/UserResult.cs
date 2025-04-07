using Domain.Models;

namespace Business.Models;

public class UserResult : ServiceResult
{
    public User? Result { get; set; }

}
