using System.ComponentModel.DataAnnotations;

namespace CinemaWebApi.DTOs.Requests
{
    public class FoodCategoryRequest
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Slug không được để trống")]
        [MaxLength(50)]
        public string Slug { get; set; } = null!;

        public string? IconUrl { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class FoodItemRequest
    {
        [Required(ErrorMessage = "Phải chọn danh mục đồ ăn")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên món không được để trống")]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá không hợp lệ")]
        public decimal BasePrice { get; set; }

        public int? Calories { get; set; }
        public bool IsCombo { get; set; } = false;
        public bool IsAvailable { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
    }
    // 3. DTO MỚI CHO COMBO
    public class FoodComboItemRequest
    {
        [Required] public int ItemId { get; set; }
        [Required][Range(1, 100)] public int Quantity { get; set; } = 1;
    }
}