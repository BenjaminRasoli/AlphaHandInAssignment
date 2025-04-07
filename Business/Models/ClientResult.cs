using Domain.Models;

namespace Business.Models;

public class ClientResult : ServiceResult
{

}

public class ClientResult<T> : ClientResult
{
    public T? Result { get; set; }

}