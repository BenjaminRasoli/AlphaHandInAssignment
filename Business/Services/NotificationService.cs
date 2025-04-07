using Business.Models;
using Data.Entitites;
using Data.Models;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using System.Linq.Expressions;



namespace Business.Services;

public interface INotificationService
{
    Task<NotificationResult> AddNotificationAsync(NotificationFormData formData);
    Task DismissNotificationAsync(string notificationId, string userId);
    Task<NotificationResult<IEnumerable<Notification>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 10);
}

public class NotificationService(INotificationRepository notificationRepository, INotificationTypeRepository notificationTypeRepository, INotificationTargetRepository notificationTargetRepository, IUserDismissedNotificationRepository userDismissedNotificationRepository) : INotificationService
{
    private readonly INotificationRepository _notificationRepository = notificationRepository;
    private readonly INotificationTypeRepository _notificationTypeRepository = notificationTypeRepository;
    private readonly INotificationTargetRepository _notificationTargetRepository = notificationTargetRepository;
    private readonly IUserDismissedNotificationRepository _userDismissedNotificationRepository = userDismissedNotificationRepository;


    public async Task<NotificationResult> AddNotificationAsync(NotificationFormData formData)
    {
        if (formData == null)
            return new NotificationResult 
            { 
                Succeded = false,
                StatusCode = 400
            };

        if (string.IsNullOrEmpty(formData.Image))
        {
            switch (formData.NotificationTypeId)
            {
                case 1:
                    formData.Image = "/images/profiles/user-template.svg";
                    break;

                case 2:
                    formData.Image = "/images/projects/project-template.svg";
                    break;
            }
        }

        var notificationEntity = formData.MapTo<NotificationEntity>();
        var result = await _notificationRepository.AddAsync(notificationEntity);

        if (result.Succeded)
        {
            await _notificationRepository.GetLatestNotification();
        }


        return result.Succeded
            ? new NotificationResult 
            {
                Succeded = true,
                StatusCode = 200
            }
            : new NotificationResult 
            { 
                Succeded = false,
                StatusCode = result.StatusCode,
                Error = result.Error
            };
    }


    public async Task<NotificationResult<IEnumerable<Notification>>> GetNotificationsAsync(string userId, string? roleName = null, int take = 10)
    {
        var adminTargetName = "Admin";
        var dismissedNotificationResult = await _userDismissedNotificationRepository.GetNotificationsIdsAsync(userId);
        var dismissedNotificationIds = dismissedNotificationResult.Result;

        Expression<Func<NotificationEntity, NotificationEntity>> selector = x => x;

        var notificationResult = (!string.IsNullOrEmpty(roleName) && roleName == adminTargetName)
            ? await _notificationRepository.GetAllAsync(
                selector, 
                orderByDescending: true,
                sortByColumn: x => x.CreateDate,
                filterBy: x => !dismissedNotificationIds!.Contains(x.Id),
                take: take)

            : await _notificationRepository.GetAllAsync(
                selector, 
                orderByDescending: true,
                sortByColumn: x => x.CreateDate,
                filterBy: x => !dismissedNotificationIds!.Contains(x.Id) && x.NotificationTarget.TargetName != adminTargetName,
                take: take,
                includes: x => x.NotificationTarget);

        if (!notificationResult.Succeded)
            return new NotificationResult<IEnumerable<Notification>>
            {
                Succeded = false,
                StatusCode = 404
            };

        var notifications = notificationResult.Result!.Select(entity => entity.MapTo<Notification>());
        return new NotificationResult<IEnumerable<Notification>>
        {
            Succeded = true,
            StatusCode = 200,
            Result = notifications
        };
    }



    public async Task DismissNotificationAsync(string notificationId, string userId)
    {
        var exists = await _userDismissedNotificationRepository.ExistsAsync(x => x.NotificationId == notificationId && x.UserId == userId);
        if (!exists.Succeded)
        {
            var entity = new UserDismissedNotificationEntity
            {
                NotificationId = notificationId,
                UserId = userId
            };

            await _userDismissedNotificationRepository.AddAsync(entity);
        }
    }
}