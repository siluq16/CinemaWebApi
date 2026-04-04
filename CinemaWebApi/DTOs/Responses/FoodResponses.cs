namespace CinemaWebApi.DTOs.Responses
{
    public class FoodCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class FoodItemResponse
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public int? Calories { get; set; }
        public bool IsCombo { get; set; }
        public bool IsAvailable { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class FoodComboItemResponse
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = null!; // Lấy tên món cho FE dễ hiển thị
        public int Quantity { get; set; }
    }
}
