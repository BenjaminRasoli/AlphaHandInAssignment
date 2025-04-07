using Domain.Models;

namespace Data.Models
{
    public class NotificationResult : RepositoryResult<Notification>
    {
    }

    public class NotificationResult<T> : RepositoryResult<T>
    {
    }
}
