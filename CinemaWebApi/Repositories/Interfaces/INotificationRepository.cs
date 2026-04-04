using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int limit = 20);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<Notification?> GetByIdAsync(Guid id);
        Task CreateNotificationAsync(Notification notification);
        void Update(Notification notification);
        Task<bool> SaveChangesAsync();
    }
}
