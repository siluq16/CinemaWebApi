using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;
using CinemaWebApi.Helpers;

namespace CinemaWebApi.Services.Implementations
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly IShowtimeRepository _showtimeRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IScreeningRoomRepository _roomRepository;
        private readonly ISeatLayoutRepository _seatRepository;
        private readonly IPricingRepository _pricingRepository;

        public ShowtimeService(
            IShowtimeRepository showtimeRepository,
            IMovieRepository movieRepository,
            IScreeningRoomRepository roomRepository,
            ISeatLayoutRepository seatRepository,
            IPricingRepository pricingRepository) // Gán vào đây
        {
            _showtimeRepository = showtimeRepository;
            _movieRepository = movieRepository;
            _roomRepository = roomRepository;
            _seatRepository = seatRepository;
            _pricingRepository = pricingRepository;
        }

        private ShowtimeResponse MapToResponse(Showtime s)
        {
            return new ShowtimeResponse
            {
                Id = s.Id,
                MovieId = s.MovieId,
                MovieTitle = s.Movie?.Title ?? "N/A",
                DurationMinutes = s.Movie?.DurationMinutes ?? 0,
                RoomId = s.RoomId,
                RoomName = s.Room?.Name ?? "N/A",
                CinemaName = s.Room?.Cinema?.Name ?? "N/A",
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Language = s.Language,
                SubtitleType = s.SubtitleType,
                ScreenFormat = s.ScreenFormat,
                IsCancelled = s.IsCancelled,
                Notes = s.Notes,
                TotalSeats = s.Room?.SeatLayouts.Count(seat => seat.IsActive) ?? 0,
                SeatsTaken = s.Bookings.Count(t => t.Status == "Paid" || t.Status == "Booked")
            };
        }

        public async Task<IEnumerable<ShowtimeResponse>> GetAllShowtimesAsync()
        {
            var showtimes = await _showtimeRepository.GetAllAsync();
            return showtimes.Select(MapToResponse);
        }
        public async Task<IEnumerable<ShowtimeResponse>> GetUpcomingShowtimesAsync()
        {
            var views = await _showtimeRepository.GetUpcomingShowtimesAsync();

            return views.Select(v => new ShowtimeResponse
            {
                Id = v.ShowtimeId,
                MovieId = v.MovieId,
                MovieTitle = v.Title,
                DurationMinutes = v.DurationMinutes,
                RoomId = v.RoomId,
                RoomName = v.RoomName,
                CinemaName = v.CinemaName, 
                StartTime = v.StartTime,
                EndTime = v.EndTime,
                Language = v.Language,
                SubtitleType = v.SubtitleType,
                ScreenFormat = v.ScreenFormat,
                IsCancelled = false,
                TotalSeats = v.TotalSeats,
                SeatsTaken = v.SeatsTaken ?? 0,
                // Điểm đáng tiền nhất khi dùng View:
                Notes = $"Còn trống: {v.TotalSeats - v.SeatsTaken}/{v.TotalSeats} ghế",
            });
        }

        public async Task<ShowtimeResponse?> GetShowtimeByIdAsync(Guid id)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);
            if (showtime == null) return null;
            return MapToResponse(showtime);
        }

        public async Task<ShowtimeResponse> CreateShowtimeAsync(CreateShowtimeRequest request)
        {
            var movie = await _movieRepository.GetByIdAsync(request.MovieId);
            if (movie == null) throw new Exception("Không tìm thấy bộ phim.");
            var room = await _roomRepository.GetByIdAsync(request.RoomId);
            if (room == null) throw new Exception("Không tìm thấy phòng chiếu.");

            var endTime = request.StartTime.AddMinutes(movie.DurationMinutes + 15);

            bool isOverlap = await _showtimeRepository.HasOverlapAsync(request.RoomId, request.StartTime, endTime);
            if (isOverlap)
            {
                throw new Exception("Phòng chiếu đã có lịch bận trong khoảng thời gian này!");
            }

            var showtime = new Showtime
            {
                MovieId = request.MovieId,
                RoomId = request.RoomId,
                StartTime = request.StartTime,
                EndTime = endTime,
                Language = request.Language,
                SubtitleType = request.SubtitleType,
                ScreenFormat = request.ScreenFormat,
                IsCancelled = false,
                Notes = request.Notes,
                CreatedAt = DateTime.Now
            };


            await _showtimeRepository.AddShowTimeAsync(showtime);
            await _showtimeRepository.SaveChangesAsync();

            return await GetShowtimeByIdAsync(showtime.Id) ?? throw new Exception("Lỗi khi load lại dữ liệu");
        }

        public async Task<IEnumerable<ShowtimeSeatResponse>?> GetShowtimeSeatsAsync(Guid showtimeId)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(showtimeId);
            if (showtime == null) return null;

            var allSeats = await _seatRepository.GetByRoomIdAsync(showtime.RoomId);
            var bookedSeatIds = await _showtimeRepository.GetBookedSeatIdsAsync(showtimeId);

            // 1. TẢI BỘ LUẬT GIÁ
            var activeRules = await _pricingRepository.GetActiveRulesAsync();
            var isHoliday = await _pricingRepository.IsHolidayAsync(showtime.StartTime.Date);
            var dayOfWeek = ((int)showtime.StartTime.DayOfWeek).ToString();
            var timeOfDay = TimeOnly.FromDateTime(showtime.StartTime);

            var result = new List<ShowtimeSeatResponse>();
            foreach (var seat in allSeats)
            {
                if (!seat.IsActive) continue;

                // 2. TÍNH GIÁ CHO TỪNG GHẾ THEO MA TRẬN
                decimal price = 0;
                try
                {
                    price = PricingHelper.CalculateSeatPrice(seat.SeatType, showtime.ScreenFormat, isHoliday, dayOfWeek, timeOfDay, activeRules);
                }
                catch
                {
                    price = 0;
                }

                result.Add(new ShowtimeSeatResponse
                {
                    SeatId = seat.Id,
                    RowLabel = seat.RowLabel,
                    SeatNumber = seat.SeatNumber,
                    SeatType = seat.SeatType,
                    XPosition = seat.XPosition,
                    YPosition = seat.YPosition,
                    IsBooked = bookedSeatIds.Contains(seat.Id),
                    Price = price // <-- TRẢ VỀ GIÁ ĐỘNG CHO FRONTEND
                });
            }

            return result;
        }

        public async Task<IEnumerable<ShowtimeResponse>> GetShowtimesByDateAsync(DateTime date, Guid? movieId = null, int? cinemaId = null)
        {
            var views = await _showtimeRepository.GetShowtimesByDateAsync(date, movieId, cinemaId);

            return views.Select(v => new ShowtimeResponse
            {
                Id = v.ShowtimeId,
                MovieId = v.MovieId,
                MovieTitle = v.Title,
                DurationMinutes = v.DurationMinutes,
                RoomId = v.RoomId,
                RoomName = v.RoomName,
                CinemaName = v.CinemaName,
                StartTime = v.StartTime,
                EndTime = v.EndTime,
                Language = v.Language,
                SubtitleType = v.SubtitleType,
                ScreenFormat = v.ScreenFormat,
                IsCancelled = false,
                TotalSeats = v.TotalSeats,
                SeatsTaken = v.SeatsTaken ?? 0,
                Notes = $"Còn trống: {v.TotalSeats - v.SeatsTaken}/{v.TotalSeats} ghế",
            });
        }
        public async Task<bool> CancelShowtimeAsync(Guid id)
        {
            var showtime = await _showtimeRepository.GetByIdAsync(id);
            if (showtime == null) return false;

            showtime.IsCancelled = true;
            _showtimeRepository.UpShowTimedate(showtime);
            await _showtimeRepository.SaveChangesAsync();
            return true;
        }
    }
}