using System.Security.Claims;
using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Phải đăng nhập mới được thanh toán
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

                var userId = Guid.Parse(userIdString);

                var isSuccess = await _paymentService.ProcessPaymentAsync(request, userId);
                if (isSuccess)
                {
                    return Ok(new
                    {
                        message = "Thanh toán thành công. Chúc bạn xem phim vui vẻ!",
                        bookingId = request.BookingId // Cực kỳ quan trọng
                    });
                }

                return BadRequest(new { message = "Thanh toán thất bại." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("create-vnpay-url/{bookingId:guid}")]
        [Authorize]
        public async Task<IActionResult> CreateVnPayUrl(Guid bookingId)
        {
            try
            {
                string url = await _paymentService.CreateVnPayPaymentUrlAsync(bookingId, HttpContext);
                return Ok(new { paymentUrl = url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("vnpay-ipn")]
        [AllowAnonymous]
        public async Task<IActionResult> VnPayIpn()
        {
            try
            {
                var resultJson = await _paymentService.ProcessVnPayIpnAsync(Request.Query);

                return Content(resultJson, "application/json");
            }
            catch (Exception ex)
            {
                return Content("{\"RspCode\":\"99\",\"Message\":\"Unknown error\"}", "application/json");
            }
        }
    }
}