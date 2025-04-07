using Data.Contexts;
using Data.Entitites;
using Data.Models;
using Domain.Extensions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public interface INotificationRepository : IBaseRepository<NotificationEntity, Notification>
{
    Task<NotificationResult<Notification>> GetLatestNotification();
}


public class NotificationRepository(AppDbContext context) : BaseRepository<NotificationEntity, Notification>(context), INotificationRepository
{
    public async Task<NotificationResult<Notification>> GetLatestNotification()
    {
        var entity = await _table.OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync();
        var notification = entity!.MapTo<Notification>();
        return new NotificationResult<Notification> 
        { 
            Succeded = true,
            StatusCode = 200,
            Result = notification
        };
    }
}
