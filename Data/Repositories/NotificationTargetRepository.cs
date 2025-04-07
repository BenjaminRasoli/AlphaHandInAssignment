using Data.Contexts;
using Data.Entitites;
using Domain.Models;

namespace Data.Repositories;

public interface INotificationTargetRepository : IBaseRepository<NotificationTargetEntity, Notification>
{

}


public class NotificationTargetRepository(AppDbContext context) : BaseRepository<NotificationTargetEntity, Notification>(context), INotificationTargetRepository
{

}
