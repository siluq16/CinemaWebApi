using CinemaWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(CinemaWebApiContext context)
        {
            // 1. TẠO TÀI KHOẢN admin
            if (!await context.Users.AnyAsync(u => u.Email == "admin@cinema.com"))
            {
                var adminUser = new User
                {
                    FullName = "Quản trị viên Hệ thống",
                    Email = "admin@cinema.com",
                    Phone = "0987654321",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), // Pass: admin@123
                    Role = "admin",      // Check constraint: 'admin', 'staff', 'customer'
                    IsVerified = true,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await context.Users.AddAsync(adminUser);
            }

            // 2. TẠO RẠP CHIẾU & PHÒNG CHIẾU
            if (!await context.Cinemas.AnyAsync())
            {
                var cinema = new Cinema
                {
                    Name = "Cinema Center Landmark",
                    Address = "Tầng 5, Tòa nhà Landmark, 72 Tôn Thất Thuyết",
                    City = "Hà Nội",
                    District = "Cầu Giấy",
                    Phone = "19001234",
                    Email = "support@cinema.com",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                await context.Cinemas.AddAsync(cinema);
                await context.SaveChangesAsync(); // Lưu để lấy ID rạp (Identity INT)

                // Tạo 1 phòng chiếu IMAX
                var roomImax = new ScreeningRoom
                {
                    CinemaId = cinema.Id,
                    Name = "Phòng chiếu số 1",
                    RoomType = "IMAX", // Check constraint: '2D', '3D', 'IMAX', '4DX'...
                    TotalSeats = 100,
                    IsActive = true
                };
                await context.ScreeningRooms.AddAsync(roomImax);
                await context.SaveChangesAsync(); // Lưu để lấy ID phòng

                // 3. TẠO MA TRẬN GHẾ CHO PHÒNG CHIẾU (100 ghế)
                var seats = new List<SeatLayout>();
                string[] rows = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

                foreach (var row in rows)
                {
                    for (int num = 1; num <= 10; num++)
                    {
                        // Logic phân loại: A-E là Standard, F-I là VIP, J là Couple
                        string seatType = "standard";
                        if ("FGHI".Contains(row)) seatType = "vip";
                        if (row == "J") seatType = "couple";

                        seats.Add(new SeatLayout
                        {
                            RoomId = roomImax.Id,
                            RowLabel = row,
                            SeatNumber = num,
                            SeatType = seatType, // Check constraint: 'standard', 'vip', 'couple'...
                            IsActive = true
                        });
                    }
                }
                await context.SeatLayouts.AddRangeAsync(seats);
            }

            // 4. TẠO PHIM MẪU
            if (!await context.Movies.AnyAsync())
            {
                var movies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "Avenger: Hồi Kết",
                        OriginalTitle = "Avengers: Endgame",
                        Description = "Trận chiến cuối cùng bảo vệ trái đất khỏi Thanos...",
                        DurationMinutes = 181,
                        ReleaseDate = new DateOnly(2025, 4, 26),
                        Language = "en", // DB varchar(10)
                        Country = "Mỹ",
                        Status = "now_showing", // Check constraint: 'now_showing', 'coming_soon'...
                        AvgRating = 0.0m,
                        ReviewCount = 0,
                        AgeRating = 13, // DB là kiểu INT
                        IsFeatured = true,
                        PosterUrl = "https://example.com/poster-avenger.jpg",
                        TrailerUrl = "https://youtube.com/...",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new Movie
                    {
                        Title = "Doraemon: Bản giao hưởng Trái Đất",
                        OriginalTitle = "Doraemon The Movie 2024",
                        Description = "Chuyến phiêu lưu âm nhạc cùng Mèo Ú",
                        DurationMinutes = 115,
                        ReleaseDate = new DateOnly(2025, 5, 24),
                        Language = "vi",
                        Country = "Nhật Bản",
                        Status = "coming_soon",
                        AvgRating = 0.0m,
                        ReviewCount = 0,
                        AgeRating = 0, // Dành cho mọi lứa tuổi
                        IsFeatured = true,
                        PosterUrl = "https://example.com/poster-doraemon.jpg",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };
                await context.Movies.AddRangeAsync(movies);
            }

            // 5. TẠO LUẬT GIÁ VÉ ĐỘNG
            if (!await context.PricingRules.AnyAsync())
            {
                var rules = new List<PricingRule>
                {
                    new PricingRule { RuleName = "Giá vé Standard cơ bản", SeatType = "standard", BasePrice = 70000, Priority = 1, IsActive = true, CreatedAt = DateTime.Now },
                    new PricingRule { RuleName = "Giá vé VIP cơ bản", SeatType = "vip", BasePrice = 90000, Priority = 1, IsActive = true, CreatedAt = DateTime.Now },
                    new PricingRule { RuleName = "Giá vé Couple cơ bản", SeatType = "couple", BasePrice = 150000, Priority = 1, IsActive = true, CreatedAt = DateTime.Now },
                    
                    // Phụ thu ghế xem phim IMAX (Định dạng chiếu)
                    new PricingRule { RuleName = "Phụ thu định dạng IMAX", ScreenFormat = "IMAX_2D", BasePrice = 120000, Priority = 3, IsActive = true, CreatedAt = DateTime.Now },
                    
                    // Phụ thu cuối tuần (Thứ 7, CN) - Ưu tiên cao nhất để đè lên giá cơ bản
                    new PricingRule { RuleName = "Phụ thu cuối tuần Standard", SeatType = "standard", DayOfWeekFilter = "0,6", BasePrice = 90000, Priority = 5, IsActive = true, CreatedAt = DateTime.Now },
                    new PricingRule { RuleName = "Phụ thu cuối tuần VIP", SeatType = "vip", DayOfWeekFilter = "0,6", BasePrice = 110000, Priority = 5, IsActive = true, CreatedAt = DateTime.Now }
                };
                await context.PricingRules.AddRangeAsync(rules);
            }

            // 6. TẠO ĐỒ ĂN / BẮP NƯỚC (Bổ sung để làm luồng Booking xịn hơn)
            if (!await context.FoodCategories.AnyAsync())
            {
                var popcornCat = new FoodCategory { Name = "Bắp rang", Slug = "bap-rang", DisplayOrder = 1, IsActive = true };
                var drinkCat = new FoodCategory { Name = "Nước uống", Slug = "nuoc-uong", DisplayOrder = 2, IsActive = true };
                var comboCat = new FoodCategory { Name = "Combo", Slug = "combo", DisplayOrder = 3, IsActive = true };

                await context.FoodCategories.AddRangeAsync(popcornCat, drinkCat, comboCat);
                await context.SaveChangesAsync();

                var foods = new List<FoodItem>
                {
                    new FoodItem { CategoryId = popcornCat.Id, Name = "Bắp rang phô mai (Lớn)", BasePrice = 65000, IsCombo = false, IsAvailable = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new FoodItem { CategoryId = drinkCat.Id, Name = "Pepsi Tươi (Lớn)", BasePrice = 35000, IsCombo = false, IsAvailable = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                    new FoodItem { CategoryId = comboCat.Id, Name = "Combo 1 Bắp 2 Nước", BasePrice = 110000, IsCombo = true, IsAvailable = true, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }
                };
                await context.FoodItems.AddRangeAsync(foods);
            }

            // LƯU TẤT CẢ VÀO DATABASE
            await context.SaveChangesAsync();
        }
    }
}