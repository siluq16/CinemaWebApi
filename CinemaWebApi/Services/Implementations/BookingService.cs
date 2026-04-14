using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;
using CinemaWebApi.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IPricingRepository _pricingRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IShowtimeRepository _showtimeRepo;
        private readonly IUserRepository _userRepo;
        private readonly ISeatLayoutRepository _seatRepo;
        private readonly IFoodRepository _foodRepo;
        private readonly IPromotionRepository _promotionRepo;
        private readonly INotificationService _notiService;

        public BookingService(
            IBookingRepository bookingRepo,
            IShowtimeRepository showtimeRepo,
            IUserRepository userRepo,
            ISeatLayoutRepository seatRepo,
            IFoodRepository foodRepo,
            IPromotionRepository promotionRepo,
            IPricingRepository pricingRepo,
            INotificationService notiService) // Gán giá trị
        {
            _bookingRepo = bookingRepo;
            _showtimeRepo = showtimeRepo;
            _userRepo = userRepo;
            _seatRepo = seatRepo;
            _foodRepo = foodRepo;
            _promotionRepo = promotionRepo;
            _pricingRepo = pricingRepo; // Khởi tạo
            _notiService = notiService;
        }

        public async Task<List<BookingResponse>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepo.GetAllBooking();

            var response = bookings.Select(b => new BookingResponse
            {
                Id = b.Id,
                BookingCode = b.BookingCode,
                UserName = b.User?.FullName ?? "Khách vô danh",
                MovieTitle = b.Showtime?.Movie?.Title ?? "N/A",
                CinemaName = b.Showtime?.Room?.Cinema?.Name ?? "N/A",
                RoomName = b.Showtime?.Room?.Name ?? "N/A",
                StartTime = b.Showtime?.StartTime ?? DateTime.MinValue,
                Status = b.Status,
                TicketTotal = b.TotalAmount,
                FoodTotal = b.FoodAmount,
                SubTotal = b.TotalAmount + b.FoodAmount,
                DiscountAmount = b.DiscountAmount,
                FinalAmount = b.FinalAmount,
                CreatedAt = b.CreatedAt,

                Seats = b.BookingSeats.Select(s => new BookingSeatResponse
                {
                    SeatId = s.SeatId,
                    RowLabel = s.Seat?.RowLabel ?? "",
                    SeatNumber = s.Seat?.SeatNumber ?? 0,
                    Price = s.Price
                }).ToList(),

                FoodItems = b.FoodOrders.SelectMany(fo => fo.FoodOrderItems).Select(f => new BookingFoodResponse
                {
                    FoodItemId = f.FoodItemId,
                    FoodName = f.FoodItem?.Name ?? "",
                    Quantity = f.Quantity,
                    UnitPrice = f.UnitPrice,
                    SubTotal = f.Subtotal
                }).ToList()
            }).OrderByDescending(b => b.CreatedAt).ToList(); 

            return response;
        }

        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            var user = await _userRepo.GetByIdAsync(request.UserId);
            if (user == null) throw new Exception("Người dùng không tồn tại.");

            var showtime = await _showtimeRepo.GetByIdAsync(request.ShowtimeId);
            if (showtime == null || showtime.IsCancelled)
                throw new Exception("Suất chiếu không tồn tại hoặc đã bị hủy.");

            if (showtime.StartTime < DateTime.Now)
                throw new Exception("Suất chiếu này đã bắt đầu, không thể đặt vé.");

            var room = showtime.Room;
            if (room == null || room.Cinema == null)
                throw new Exception("Lỗi dữ liệu phòng chiếu/rạp.");

            bool isAvailable = await _bookingRepo.AreSeatsAvailableAsync(request.ShowtimeId, request.SeatIds);
            if (!isAvailable)
                throw new Exception("Xin lỗi, một hoặc nhiều ghế bạn chọn đã có người khác nhanh tay đặt mất rồi.");

            var allRoomSeats = await _seatRepo.GetByRoomIdAsync(showtime.RoomId);
            var selectedSeats = allRoomSeats.Where(s => request.SeatIds.Contains(s.Id)).ToList();

            if (selectedSeats.Count != request.SeatIds.Count)
                throw new Exception("Có ghế không hợp lệ trong phòng chiếu này.");

            decimal ticketTotal = 0;
            var bookingSeats = new List<BookingSeat>();
            var activeRules = await _pricingRepo.GetActiveRulesAsync();
            var isHoliday = await _pricingRepo.IsHolidayAsync(showtime.StartTime.Date);
            var dayOfWeek = ((int)showtime.StartTime.DayOfWeek).ToString(); // 0 là Chủ Nhật, 1-6 là Thứ 2-7
            var timeOfDay = TimeOnly.FromDateTime(showtime.StartTime);

            foreach (var seat in selectedSeats)
            {
                decimal price = PricingHelper.CalculateSeatPrice(seat.SeatType, showtime.ScreenFormat, isHoliday, dayOfWeek, timeOfDay, activeRules);
                ticketTotal += price;

                bookingSeats.Add(new BookingSeat
                {
                    SeatId = seat.Id,
                    Price = price, 
                    Status = "held"
                });
            }

            //decimal foodTotal = 0;
            //FoodOrder? foodOrder = null;
            //var foodOrderItems = new List<FoodOrderItem>();

            //if (request.FoodItems != null && request.FoodItems.Any())
            //{
            //    foodOrder = new FoodOrder
            //    {
            //        CinemaId = room.CinemaId,
            //        Status = "pending",
            //        CreatedAt = DateTime.Now,
            //        UpdatedAt = DateTime.Now
            //    };

            //    foreach (var reqFood in request.FoodItems)
            //    {
            //        var foodItemDb = await _foodRepo.GetItemByIdAsync(reqFood.FoodItemId);
            //        if (foodItemDb == null || !foodItemDb.IsAvailable)
            //            throw new Exception($"Món ăn ID {reqFood.FoodItemId} không tồn tại hoặc đã hết.");

            //        decimal subTotal = foodItemDb.BasePrice * reqFood.Quantity;
            //        foodTotal += subTotal;

            //        foodOrderItems.Add(new FoodOrderItem
            //        {
            //            FoodItemId = foodItemDb.Id,
            //            Quantity = reqFood.Quantity,
            //            UnitPrice = foodItemDb.BasePrice,
            //            Subtotal = subTotal,
            //            Status = "pending"
            //        });
            //    }
            //    foodOrder.TotalAmount = foodTotal;
            //}

            var booking = new Booking
            {
                UserId = request.UserId,
                ShowtimeId = request.ShowtimeId,
                TotalAmount = ticketTotal,
                Status = "pending",
                BookingCode = GenerateBookingCode(), 
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                FinalAmount = ticketTotal, 
                ExpiresAt = DateTime.Now.AddMinutes(10) // BỔ SUNG DÒNG NÀY (Hủy đơn sau 10 phút)
            };

            try
            {
                await _bookingRepo.CreateBookingTransactionAsync(booking, bookingSeats);
            }
            catch (DbUpdateException)
            {
                throw new Exception("Xin lỗi, ghế bạn chọn vừa có người khác nhanh tay đặt mất. Vui lòng tải lại sơ đồ ghế!");
            }
            catch (Exception)
            {
                throw new Exception("Có lỗi xảy ra khi giữ ghế. Vui lòng thử lại.");
            }

            return await GetBookingByIdAsync(booking.Id) ?? throw new Exception("Lỗi khi tạo biên lai.");
        }

        public async Task<BookingResponse?> AddFoodToBookingAsync(Guid bookingId, List<BookingFoodItemRequest> foodRequests)
        {
            var booking = await _bookingRepo.GetBookingByIdAsync(bookingId);
            if (booking == null) throw new Exception("Không tìm thấy đơn hàng.");

            if (booking.Status != "pending")
                throw new Exception("Chỉ có thể thêm đồ ăn vào đơn hàng đang chờ thanh toán.");

            decimal foodTotal = 0;
            var foodOrder = new FoodOrder
            {
                CinemaId = booking.Showtime!.Room!.CinemaId, 
                Status = "pending",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var foodOrderItems = new List<FoodOrderItem>();

            foreach (var req in foodRequests)
            {
                var foodItemDb = await _foodRepo.GetItemByIdAsync(req.FoodItemId);
                if (foodItemDb == null || !foodItemDb.IsAvailable)
                    throw new Exception($"Món ăn ID {req.FoodItemId} không tồn tại hoặc đã hết.");

                decimal subTotal = foodItemDb.BasePrice * req.Quantity;
                foodTotal += subTotal;

                foodOrderItems.Add(new FoodOrderItem
                {
                    FoodItemId = foodItemDb.Id,
                    Quantity = req.Quantity,
                    UnitPrice = foodItemDb.BasePrice,
                    Subtotal = subTotal,
                    Status = "pending"
                });
            }
            foodOrder.TotalAmount = foodTotal;

            booking.FoodAmount += foodTotal;
            booking.FinalAmount += foodTotal;
            booking.UpdatedAt = DateTime.Now;

            await _bookingRepo.AddFoodToBookingAsync(bookingId, foodOrder, foodOrderItems, foodTotal);

            return await GetBookingByIdAsync(bookingId);
        }

        public async Task<BookingResponse?> GetBookingByIdAsync(Guid id)
        {
            var b = await _bookingRepo.GetBookingByIdAsync(id);
            if (b == null) return null;
            decimal ticketTotal = b.TotalAmount; 
            decimal foodTotal = b.FoodAmount;
            var response = new BookingResponse
            {
                Id = b.Id,
                BookingCode = b.BookingCode,
                MovieTitle = b.Showtime?.Movie?.Title ?? "N/A",
                CinemaName = b.Showtime?.Room?.Cinema?.Name ?? "N/A",
                RoomName = b.Showtime?.Room?.Name ?? "N/A",
                StartTime = b.Showtime?.StartTime ?? DateTime.MinValue,
                Status = b.Status,
                TicketTotal = ticketTotal,
                FoodTotal = foodTotal,
                SubTotal = ticketTotal + foodTotal ,
                DiscountAmount = b.DiscountAmount,
                FinalAmount = b.FinalAmount,
                CreatedAt = b.CreatedAt,

                Seats = b.BookingSeats.Select(s => new BookingSeatResponse
                {
                    SeatId = s.SeatId,
                    RowLabel = s.Seat?.RowLabel ?? "",
                    SeatNumber = s.Seat?.SeatNumber ?? 0,
                    Price = s.Price
                }).ToList(),

                FoodItems = b.FoodOrders.FirstOrDefault()?.FoodOrderItems.Select(f => new BookingFoodResponse
                {
                    FoodItemId = f.FoodItemId,
                    FoodName = f.FoodItem?.Name ?? "",
                    Quantity = f.Quantity,
                    UnitPrice = f.UnitPrice,
                    SubTotal = f.Subtotal
                }).ToList() ?? new List<BookingFoodResponse>()
            };

            return response;
        }

        public async Task<BookingResponse?> ApplyPromotionAsync(Guid bookingId, Guid userId, string promotionCode)
        {
            using var transaction = await _bookingRepo.BeginTransactionAsync();

            try
            {
                var booking = await _bookingRepo.GetBookingWithSeatsAsync(bookingId);

                if (booking == null) throw new Exception("Không tìm thấy đơn hàng.");
                if (booking.UserId != userId) throw new Exception("Bạn không có quyền thao tác trên đơn hàng này.");
                if (booking.Status != "pending") throw new Exception("Chỉ áp dụng mã cho đơn hàng chờ thanh toán.");

                var promo = await _promotionRepo.GetByCodeAsync(promotionCode);

                if (promo == null || !promo.IsActive)
                    throw new Exception("Mã khuyến mãi không tồn tại hoặc đã bị khóa.");

                var now = DateTime.Now;
                if (now < promo.ValidFrom || now > promo.ValidTo)
                    throw new Exception("Mã khuyến mãi chưa tới ngày áp dụng hoặc đã hết hạn.");

                if (promo.MaxUses.HasValue && promo.UsedCount >= promo.MaxUses.Value)
                    throw new Exception("Mã khuyến mãi đã hết lượt sử dụng trên toàn hệ thống.");

                var userUsageCount = await _promotionRepo.GetUserUsageCountAsync(userId, promo.Id);

                if (userUsageCount >= promo.MaxUsesPerUser)
                    throw new Exception($"Bạn đã hết lượt sử dụng mã này (Tối đa {promo.MaxUsesPerUser} lần/người).");

                var existingPromo = await _promotionRepo.HasBookingAppliedPromotionAsync(bookingId);
                if (existingPromo)
                    throw new Exception("Đơn hàng này đã được áp dụng mã giảm giá. Vui lòng gỡ mã cũ trước khi nhập mã mới.");

                decimal currentTotal = booking.TotalAmount + booking.FoodAmount;

                if (currentTotal < promo.MinOrderValue)
                    throw new Exception($"Đơn hàng phải từ {promo.MinOrderValue:N0}đ mới được áp dụng mã này.");

                decimal discount = 0;

                if (promo.DiscountType == "percentage")
                {
                    discount = currentTotal * (promo.DiscountValue / 100m);
                    if (promo.MaxDiscountAmount.HasValue && discount > promo.MaxDiscountAmount.Value)
                    {
                        discount = promo.MaxDiscountAmount.Value;
                    }
                }
                else if (promo.DiscountType == "fixed_amount")
                {
                    discount = promo.DiscountValue;
                }

                if (discount > currentTotal) discount = currentTotal;


                booking.DiscountAmount = discount;
                booking.FinalAmount = currentTotal - discount; 
                booking.UpdatedAt = DateTime.Now;
                _bookingRepo.Update(booking); 

                await _promotionRepo.AddBookingPromotionAsync(new BookingPromotion
                {
                    BookingId = booking.Id,
                    PromotionId = promo.Id,
                    AppliedDiscount = discount
                });

                await _promotionRepo.AddUserPromotionUsageAsync(new UserPromotionUsage 
                {
                    UserId = userId,
                    PromotionId = promo.Id,
                    BookingId = booking.Id,
                    UsedAt = DateTime.Now
                });

                promo.UsedCount += 1;
                _promotionRepo.Update(promo); 

                await _bookingRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetBookingByIdAsync(booking.Id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<BookingResponse?> RemovePromotionAsync(Guid bookingId, Guid userId)
        {
            using var transaction = await _bookingRepo.BeginTransactionAsync();

            try
            {
                var booking = await _bookingRepo.GetBookingWithSeatsAsync(bookingId);

                if (booking == null) throw new Exception("Không tìm thấy đơn hàng.");
                if (booking.UserId != userId) throw new Exception("Bạn không có quyền thao tác trên đơn hàng này.");
                if (booking.Status != "pending") throw new Exception("Chỉ gỡ mã được cho đơn hàng chờ thanh toán.");
                if (booking.DiscountAmount == 0)
                    return await GetBookingByIdAsync(bookingId);

                await _promotionRepo.RevertPromotionUsageAsync(booking.Id);

                booking.DiscountAmount = 0;
                booking.FinalAmount = booking.TotalAmount + booking.FoodAmount;
                booking.UpdatedAt = DateTime.Now;

                _bookingRepo.Update(booking);

                await _bookingRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                // 3. Trả về thông tin  hàng sau khi đã gỡ mã để React hiển thị
                return await GetBookingByIdAsync(booking.Id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        // Hàm sinh mã vé ngẫu nhiên dạng V-ABC1234
        private string GenerateBookingCode()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomString = new string(Enumerable.Repeat(chars, 7)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return $"V-{randomString}";
        }

        public async Task CancelExpiredBookingsAsync()
        {
            var expiredBookings = await _bookingRepo.GetExpiredPendingBookingsAsync();
            if (!expiredBookings.Any()) return;

            foreach (var booking in expiredBookings)
            {
                using var transaction = await _bookingRepo.BeginTransactionAsync();
                try
                {
                    booking.Status = "cancelled";
                    booking.UpdatedAt = DateTime.Now;
                    _bookingRepo.Update(booking);

                    if (booking.BookingSeats.Any())
                    {
                        _bookingRepo.RemoveRange(booking.BookingSeats);
                    }

                    foreach (var foodOrder in booking.FoodOrders)
                    {
                        foodOrder.Status = "cancelled";
                        foodOrder.UpdatedAt = DateTime.Now;
                        foreach (var item in foodOrder.FoodOrderItems)
                        {
                            item.Status = "cancelled";
                        }
                    }

                    await _promotionRepo.RevertPromotionUsageAsync(booking.Id);
                    await _notiService.SendNotificationAsync(
                        booking.UserId,
                        "⚠️ Hủy vé tự động",
                        $"Đơn đặt vé {booking.BookingCode} của bạn đã bị hủy do quá hạn thanh toán 10 phút.",
                        "booking_cancelled"
                    );

                    await _bookingRepo.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }
        }
        public async Task ClearMyPendingBookingsAsync(Guid userId, Guid showtimeId)
        {
            await _bookingRepo.ClearUserPendingBookingAsync(userId, showtimeId);
        }

    }
}