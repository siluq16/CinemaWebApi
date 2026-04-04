using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notiRepo;

        public NotificationService(INotificationRepository notiRepo)
        {
            _notiRepo = notiRepo;
        }

        public async Task<IEnumerable<NotificationResponse>> GetMyNotificationsAsync(Guid userId)
        {
            var notis = await _notiRepo.GetUserNotificationsAsync(userId);
            return notis.Select(n => new NotificationResponse
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                Type = n.Type,
                Metadata = n.Metadata,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
        }

        public async Task<int> GetMyUnreadCountAsync(Guid userId)
        {
            return await _notiRepo.GetUnreadCountAsync(userId);
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var noti = await _notiRepo.GetByIdAsync(notificationId);
            if (noti == null || noti.UserId != userId) return false;

            if (!noti.IsRead)
            {
                noti.IsRead = true;
                noti.ReadAt = DateTime.Now;
                _notiRepo.Update(noti);
                await _notiRepo.SaveChangesAsync();
            }
            return true;
        }

        public async Task<bool> MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotis = (await _notiRepo.GetUserNotificationsAsync(userId, 50))
                              .Where(n => !n.IsRead).ToList();

            if (!unreadNotis.Any()) return true;

            foreach (var noti in unreadNotis)
            {
                noti.IsRead = true;
                noti.ReadAt = DateTime.Now;
                _notiRepo.Update(noti);
            }
            await _notiRepo.SaveChangesAsync();
            return true;
        }

        public async Task SendNotificationAsync(Guid userId, string title, string body, string type, string? metadata = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Body = body,
                Type = type,
                Metadata = metadata,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            await _notiRepo.CreateNotificationAsync(notification);
            await _notiRepo.SaveChangesAsync();

            // Tương lai: Tích hợp SignalR tại đây để push realtime lên web
        }
    }
}