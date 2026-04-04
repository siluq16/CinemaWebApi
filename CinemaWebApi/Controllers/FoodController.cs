using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        // ================= CATEGORIES =================

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            return Ok(await _foodService.GetAllCategoriesAsync());
        }

        [HttpPost("categories")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateCategory([FromBody] FoodCategoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _foodService.CreateCategoryAsync(request);
            return Ok(created);
        }

        [HttpPut("categories/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] FoodCategoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _foodService.UpdateCategoryAsync(id, request);
            if (updated == null) return NotFound("Không tìm thấy danh mục đồ ăn.");
            return Ok(updated);
        }

        // ================= ITEMS =================

        [HttpGet("items")]
        public async Task<IActionResult> GetAllItems()
        {
            return Ok(await _foodService.GetAllItemsAsync());
        }

        [HttpGet("items/category/{categoryId}")]
        public async Task<IActionResult> GetItemsByCategory(int categoryId)
        {
            return Ok(await _foodService.GetItemsByCategoryAsync(categoryId));
        }

        [HttpGet("items/{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _foodService.GetItemByIdAsync(id);
            if (item == null) return NotFound("Không tìm thấy món ăn.");
            return Ok(item);
        }

        [HttpPost("items")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateItem([FromBody] FoodItemRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _foodService.CreateItemAsync(request);
            if (created == null) return BadRequest("Danh mục (CategoryId) không tồn tại.");
            return Ok(created);
        }

        [HttpPut("items/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] FoodItemRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _foodService.UpdateItemAsync(id, request);
            if (updated == null) return NotFound("Không tìm thấy món ăn hoặc Danh mục không hợp lệ.");
            return Ok(updated);
        }

        [HttpDelete("items/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var isDeleted = await _foodService.DeleteItemAsync(id);
            if (!isDeleted) return NotFound("Không tìm thấy món ăn.");
            return Ok(new { message = "Đã ẩn món ăn thành công (Soft delete)." });
        }

        // Lấy chi tiết các món trong 1 Combo
        [HttpGet("combos/{comboId}/items")]
        public async Task<IActionResult> GetComboItems(int comboId)
        {
            var items = await _foodService.GetComboDetailAsync(comboId);
            return Ok(items);
        }

        // Cập nhật/Thêm các món vào 1 Combo
        [HttpPost("combos/{comboId}/items")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateComboItems(int comboId, [FromBody] List<FoodComboItemRequest> requests)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Xử lý logic check: Không cho phép Combo tự chứa chính nó
            if (requests.Any(r => r.ItemId == comboId))
            {
                return BadRequest(new { message = "Combo không thể tự chứa chính nó!" });
            }

            var success = await _foodService.UpdateComboDetailAsync(comboId, requests);
            if (!success) return NotFound(new { message = "Không tìm thấy món ăn, hoặc món này không phải là Combo (is_combo = false)." });

            return Ok(new { message = "Cập nhật chi tiết Combo thành công." });
        }
    }
}