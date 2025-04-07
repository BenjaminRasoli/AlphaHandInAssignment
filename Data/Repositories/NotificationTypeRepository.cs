using Data.Contexts;
using Data.Entitites;
using Domain.Models;

namespace Data.Repositories;

public interface INotificationTypeRepository : IBaseRepository<NotificationTypeEntity, Notification>
{

}

public class NotificationTypeRepository(AppDbContext context) : BaseRepository<NotificationTypeEntity, Notification>(context), INotificationTypeRepository
{

}
