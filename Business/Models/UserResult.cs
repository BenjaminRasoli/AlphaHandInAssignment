using Domain.Models;

namespace Business.Models;

//public class UserResult : ServiceResult
//{
//    public User? Result { get; set; }

//}

public class UserResult : ServiceResult
{
    public User? User { get; set; } 
    public IEnumerable<User>? Users { get; set; } 
}

public class UserResult<T> : UserResult
{
    public T? Result { get; set; }
}
