namespace CinemaWebApi.DTOs.Responses
{
    public class BookingSeatResponse
    {
        public int SeatId { get; set; }
        public string RowLabel { get; set; } = null!;
        public int SeatNumber { get; set; }
        public decimal Price { get; set; }
    }

    public class BookingFoodResponse
    {
        public int FoodItemId { get; set; }
        public string FoodName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class BookingResponse
    {
        public Guid Id { get; set; }
        public string BookingCode { get; set; } = null!; // Mã vé dạng ngắn: "V-ABC1234"
        public string UserName { get; set; } = null!;
        public string MovieTitle { get; set; } = null!;
        public string CinemaName { get; set; } = null!;
        public string RoomName { get; set; } = null!;
        public DateTime StartTime { get; set; }

        public string Status { get; set; } = null!; // pending, confirmed, cancelled

        public decimal TicketTotal { get; set; }
        public decimal FoodTotal { get; set; }
        public decimal SubTotal { get; set; }         // TỔNG TRƯỚC GIẢM (= TicketTotal + FoodTotal)
        public decimal DiscountAmount { get; set; } // Bổ sung
        public decimal FinalAmount { get; set; }    // Bổ sung

        public List<BookingSeatResponse> Seats { get; set; } = new List<BookingSeatResponse>();
        public List<BookingFoodResponse> FoodItems { get; set; } = new List<BookingFoodResponse>();

        public DateTime CreatedAt { get; set; }
    }
}