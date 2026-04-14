using Azure.Core;
using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CinemaWebApi.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IEmailService _emailService;
        private readonly IMembershipService _membershipService;
        private readonly INotificationService _notiService;
        private readonly IConfiguration _configuration; 

        public PaymentService(
            IPaymentRepository paymentRepo, IBookingRepository bookingRepo, IEmailService emailService, IConfiguration configuration, IMembershipService membershipService, INotificationService notificationService)
        {
            _paymentRepo = paymentRepo;
            _bookingRepo = bookingRepo;
            _emailService = emailService;
            _configuration = configuration;
            _membershipService = membershipService;
            _notiService = notificationService;
        }

        public async Task<bool> ProcessPaymentAsync(ProcessPaymentRequest request, Guid userId)
        {
            //await Task.Delay(2000);

            using var transaction = await _paymentRepo.BeginTransactionAsync();
            try
            {
                var booking = await _bookingRepo.GetBookingForPaymentAsync(request.BookingId);

                if (booking == null) throw new Exception("Không tìm thấy đơn hàng.");
                if (booking.UserId != userId) throw new Exception("Bạn không có quyền thanh toán đơn hàng này.");
                if (booking.Status != "pending") throw new Exception("Đơn hàng không ở trạng thái chờ thanh toán.");
                if (request.Amount < booking.FinalAmount) throw new Exception("Số tiền thanh toán không đủ.");

                var payment = new Payment
                {
                    BookingId = booking.Id,
                    TransactionId = request.TransactionId ?? Guid.NewGuid().ToString("N"),
                    Method = request.Method,
                    Amount = request.Amount,
                    Status = "success",
                    PaidAt = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
                await _paymentRepo.AddAsync(payment);

                booking.Status = "confirmed";
                booking.UpdatedAt = DateTime.Now;
                foreach (var seat in booking.BookingSeats) seat.Status = "confirmed";
                foreach (var foodOrder in booking.FoodOrders)
                {
                    foodOrder.Status = "confirmed";
                    foodOrder.UpdatedAt = DateTime.Now;
                }

                _bookingRepo.Update(booking);
                await _paymentRepo.SaveChangesAsync();
                await transaction.CommitAsync();
                if (booking.User?.Email != null)
                {
                    // Capture dữ liệu cần thiết trước khi fire-and-forget
                    var userEmail = booking.User.Email;
                    var userName = booking.User.FullName;
                    var movieTitle = booking.Showtime?.Movie?.Title;
                    var bookingCode = booking.BookingCode;
                    var finalAmount = booking.FinalAmount;
                    var userId2 = booking.UserId;
                    var bookingId = booking.Id;
                    var seatNames = string.Join(", ", booking.BookingSeats.Select(s => $"{s.Seat?.RowLabel}{s.Seat?.SeatNumber}"));
                    var cinemaName = $"{booking.Showtime?.Room?.Cinema?.Name} - {booking.Showtime?.Room?.Name}";
                    var startTime = booking.Showtime?.StartTime;

                    // Fire-and-forget: không await, lỗi không ảnh hưởng response
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            string qrCodeUrl = $"https://quickchart.io/qr?text={bookingCode}&size=250";
                            string htmlBody = BuildEmailBody(userName, finalAmount, request.Method,
                                movieTitle, qrCodeUrl, bookingCode, cinemaName, startTime, seatNames);

                            await Task.WhenAll(
                                _emailService.SendEmailAsync(userEmail, $"🎟️ Vé Phim Mới: {movieTitle}", htmlBody),
                                _membershipService.EarnPointsAsync(userId2, bookingId, finalAmount),
                                _notiService.SendNotificationAsync(userId2,
                                    "🎟️ Đặt vé thành công!",
                                    $"Bạn đã thanh toán thành công vé xem phim {movieTitle}. Mã vé: {bookingCode}",
                                    "payment_success")
                            );
                        }
                        catch (Exception ex)
                        {
                            // Log lỗi nhưng không throw — không ảnh hưởng response
                            Console.Error.WriteLine($"[PostPayment Error] BookingId={bookingId}: {ex.Message}");
                        }
                    });
                }

                //if (booking.User?.Email != null)
                //{
                //    var seatNames = string.Join(", ", booking.BookingSeats.Select(s => $"{s.Seat?.RowLabel}{s.Seat?.SeatNumber}"));

                //    string qrCodeUrl = $"https://quickchart.io/qr?text={booking.BookingCode}&size=250";

                //    string htmlBody = $@"
                //        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
                //            <h2 style='color: #E50914; text-align: center;'>VÉ XEM PHIM ĐIỆN TỬ</h2>
                //            <p>Xin chào <strong>{booking.User.FullName}</strong>,</p>
                //            <p>Cảm ơn bạn đã đặt vé. Giao dịch thanh toán <strong>{booking.FinalAmount:N0}đ</strong> qua {request.Method} đã thành công!</p>
        
                //            <div style='background: #f9f9f9; padding: 20px; border-top: 4px solid #E50914; margin: 20px 0; text-align: center;'>
                //                <h3 style='margin-top: 0; font-size: 22px;'>{booking.Showtime?.Movie?.Title}</h3>
            
                //                <div style='margin: 20px 0;'>
                //                    <img src='{qrCodeUrl}' alt='QR Code Vé' style='border: 2px solid #ccc; padding: 10px; border-radius: 10px; width: 200px; height: 200px;' />
                //                </div>

                //                <p style='margin: 5px 0;'><strong>Mã Đặt Vé:</strong> <span style='font-size: 24px; color: #E50914; font-weight: bold; letter-spacing: 2px;'>{booking.BookingCode}</span></p>
            
                //                <hr style='border: 0; border-top: 1px dashed #ccc; margin: 15px 0;' />
            
                //                <div style='text-align: left; font-size: 15px;'>
                //                    <p style='margin: 5px 0;'><strong>Rạp:</strong> {booking.Showtime?.Room?.Cinema?.Name} - {booking.Showtime?.Room?.Name}</p>
                //                    <p style='margin: 5px 0;'><strong>Suất chiếu:</strong> {booking.Showtime?.StartTime:dd/MM/yyyy HH:mm}</p>
                //                    <p style='margin: 5px 0;'><strong>Ghế:</strong> {seatNames}</p>
                //                </div>
                //            </div>
        
                //            <p style='color: #555; font-size: 13px; text-align: center;'>Vui lòng đưa Mã QR hoặc Mã Đặt Vé này cho nhân viên tại quầy để in vé giấy hoặc quét trực tiếp qua cổng kiểm soát.</p>
                //        </div>";

                //    await _emailService.SendEmailAsync(booking.User.Email, $"🎟️ Vé Phim Mới: {booking.Showtime?.Movie?.Title}", htmlBody);
                //    await _membershipService.EarnPointsAsync(booking.UserId, booking.Id, booking.FinalAmount);
                //    await _notiService.SendNotificationAsync(
                //        booking.UserId,
                //        "🎟️ Đặt vé thành công!",
                //        $"Bạn đã thanh toán thành công vé xem phim {booking.Showtime?.Movie?.Title}. Mã vé: {booking.BookingCode}",
                //        "payment_success"
                //    );
                //}

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<string> CreateVnPayPaymentUrlAsync(Guid bookingId, HttpContext context)
        {
            var booking = await _bookingRepo.GetBookingForPaymentAsync(bookingId);
            if (booking == null || booking.Status != "pending") throw new Exception("Đơn hàng không hợp lệ.");

            var vnpayConfig = _configuration.GetSection("VnPay");
            var vnpay = new Helpers.VnPayLibrary();
            var frontendUrl = "https://movie-booking-frontend-omega.vercel.app"; 
            var returnUrl = $"{frontendUrl}/booking/{bookingId}/payment";

            vnpay.AddRequestData("vnp_Version", vnpayConfig["Version"]!);
            vnpay.AddRequestData("vnp_Command", vnpayConfig["Command"]!);
            vnpay.AddRequestData("vnp_TmnCode", vnpayConfig["TmnCode"]!);

            // VNPay yêu cầu số tiền phải nhân lên 100 lần (VD: 100,000 VND -> gửi 10000000)
            vnpay.AddRequestData("vnp_Amount", ((long)(booking.FinalAmount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", vnpayConfig["CurrCode"]!);
            vnpay.AddRequestData("vnp_IpAddr", Helpers.VnPayLibrary.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", vnpayConfig["Locale"]!);

            // Truyền BookingId làm mã tham chiếu
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan ve xem phim ma " + booking.BookingCode);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnpay.AddRequestData("vnp_TxnRef", booking.Id.ToString());

            return vnpay.CreateRequestUrl(vnpayConfig["BaseUrl"]!, vnpayConfig["HashSecret"]!);
        }

        public async Task<string> ProcessVnPayIpnAsync(IQueryCollection queryData)
        {
            var vnpayConfig = _configuration.GetSection("VnPay");
            var vnpay = new Helpers.VnPayLibrary();

            foreach (var (key, value) in queryData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            string vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_SecureHash = queryData["vnp_SecureHash"].ToString();

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnpayConfig["HashSecret"]!);
            if (!checkSignature)
            {
                return "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}"; // Sai chữ ký -> Báo lỗi
            }

            Guid bookingId = Guid.Parse(vnp_TxnRef);
            var booking = await _bookingRepo.GetBookingForPaymentAsync(bookingId);

            if (booking == null) return "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
            if (booking.Status == "confirmed") return "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";

            if (vnp_ResponseCode == "00") // Thanh toán thành công từ VNPay
            {
                var processRequest = new ProcessPaymentRequest
                {
                    BookingId = bookingId,
                    Method = "vnpay",
                    Amount = booking.FinalAmount,
                    TransactionId = vnpay.GetResponseData("vnp_TransactionNo")
                };

                // Hàm này có sẵn logic gửi Email rồi nên ta tái sử dụng luôn
                await ProcessPaymentAsync(processRequest, booking.UserId);

                return "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
            }
            else
            {
                return "{\"RspCode\":\"00\",\"Message\":\"Transaction failed\"}";
            }
        }
        public bool VerifyVnPayReturn(IQueryCollection queryData)
        {
            var vnpayConfig = _configuration.GetSection("VnPay");
            var vnpay = new Helpers.VnPayLibrary();

            foreach (var (key, value) in queryData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            string vnp_SecureHash = queryData["vnp_SecureHash"].ToString();
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnpayConfig["HashSecret"]!);

            // Trả về true nếu chữ ký đúng và thanh toán thành công
            return checkSignature && vnp_ResponseCode == "00";
        }

        private string BuildEmailBody(string userName, decimal finalAmount, string method,
            string? movieTitle, string qrCodeUrl, string bookingCode,
            string cinemaName, DateTime? startTime, string seatNames)
        {
            return $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #ddd; padding: 20px; border-radius: 10px;'>
                            <h2 style='color: #E50914; text-align: center;'>VÉ XEM PHIM ĐIỆN TỬ</h2>
                            <p>Xin chào <strong>{userName}</strong>,</p>
                            <p>Cảm ơn bạn đã đặt vé. Giao dịch thanh toán <strong>{finalAmount:N0}đ</strong> qua {method} đã thành công!</p>
        
                            <div style='background: #f9f9f9; padding: 20px; border-top: 4px solid #E50914; margin: 20px 0; text-align: center;'>
                                <h3 style='margin-top: 0; font-size: 22px;'>{movieTitle}</h3>
            
                                <div style='margin: 20px 0;'>
                                    <img src='{qrCodeUrl}' alt='QR Code Vé' style='border: 2px solid #ccc; padding: 10px; border-radius: 10px; width: 200px; height: 200px;' />
                                </div>

                                <p style='margin: 5px 0;'><strong>Mã Đặt Vé:</strong> <span style='font-size: 24px; color: #E50914; font-weight: bold; letter-spacing: 2px;'>{bookingCode}</span></p>
            
                                <hr style='border: 0; border-top: 1px dashed #ccc; margin: 15px 0;' />
            
                                <div style='text-align: left; font-size: 15px;'>
                                    <p style='margin: 5px 0;'><strong>Rạp:</strong> {cinemaName}</p>
                                    <p style='margin: 5px 0;'><strong>Suất chiếu:</strong> {startTime:dd/MM/yyyy HH:mm}</p>
                                    <p style='margin: 5px 0;'><strong>Ghế:</strong> {seatNames}</p>
                                </div>
                            </div>
        
                            <p style='color: #555; font-size: 13px; text-align: center;'>Vui lòng đưa Mã QR hoặc Mã Đặt Vé này cho nhân viên tại quầy để in vé giấy hoặc quét trực tiếp qua cổng kiểm soát.</p>
                        </div>";
        }
    }
}