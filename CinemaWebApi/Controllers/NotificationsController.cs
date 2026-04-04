using System.Security.Claims;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Phải đăng nhập mới xem được thông báo
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notiService;

        public NotificationsController(INotificationService notiService)
        {
            _notiService = notiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _notiService.GetMyNotificationsAsync(userId);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var count = await _notiService.GetMyUnreadCountAsync(userId);
            return Ok(new { unreadCount = count });
        }

        [HttpPatch("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var success = await _notiService.MarkAsReadAsync(id, userId);
            if (!success) return NotFound();
            return Ok(new { message = "Đã đánh dấu đọc." });
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _notiService.MarkAllAsReadAsync(userId);
            return Ok(new { message = "Đã đánh dấu đọc tất cả." });
        }
    }
}