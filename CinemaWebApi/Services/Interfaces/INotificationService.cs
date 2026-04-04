using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponse>> GetMyNotificationsAsync(Guid userId);
        Task<int> GetMyUnreadCountAsync(Guid userId);
        Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
        Task<bool> MarkAllAsReadAsync(Guid userId);

        Task SendNotificationAsync(Guid userId, string title, string body, string type, string? metadata = null);
    }
}
