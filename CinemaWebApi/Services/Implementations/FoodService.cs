using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;

        public FoodService(IFoodRepository foodRepository)
        {
            _foodRepository = foodRepository;
        }

        // --- CATEGORY LOGIC ---
        public async Task<IEnumerable<FoodCategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _foodRepository.GetAllCategoriesAsync();
            return categories.Select(c => new FoodCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                IconUrl = c.IconUrl,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive
            });
        }

        public async Task<FoodCategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var c = await _foodRepository.GetCategoryByIdAsync(id);
            if (c == null) return null;
            return new FoodCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Slug = c.Slug,
                IconUrl = c.IconUrl,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive
            };
        }

        public async Task<FoodCategoryResponse> CreateCategoryAsync(FoodCategoryRequest request)
        {
            var category = new FoodCategory
            {
                Name = request.Name,
                Slug = request.Slug,
                IconUrl = request.IconUrl,
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive
            };
            await _foodRepository.AddCategoryAsync(category);
            await _foodRepository.SaveChangesAsync();
            return await GetCategoryByIdAsync(category.Id) ?? throw new Exception("Lỗi tạo danh mục");
        }

        public async Task<FoodCategoryResponse?> UpdateCategoryAsync(int id, FoodCategoryRequest request)
        {
            var category = await _foodRepository.GetCategoryByIdAsync(id);
            if (category == null) return null;

            category.Name = request.Name;
            category.Slug = request.Slug;
            category.IconUrl = request.IconUrl;
            category.DisplayOrder = request.DisplayOrder;
            category.IsActive = request.IsActive;

            _foodRepository.UpdateCategory(category);
            await _foodRepository.SaveChangesAsync();
            return await GetCategoryByIdAsync(id);
        }

        // --- ITEM LOGIC ---
        private FoodItemResponse MapItemToResponse(FoodItem item)
        {
            return new FoodItemResponse
            {
                Id = item.Id,
                CategoryId = item.CategoryId,
                CategoryName = item.Category?.Name ?? "N/A",
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                BasePrice = item.BasePrice,
                Calories = item.Calories,
                IsCombo = item.IsCombo,
                IsAvailable = item.IsAvailable,
                DisplayOrder = item.DisplayOrder
            };
        }

        public async Task<IEnumerable<FoodItemResponse>> GetAllItemsAsync()
        {
            var items = await _foodRepository.GetAllItemsAsync();
            return items.Select(MapItemToResponse);
        }

        public async Task<IEnumerable<FoodItemResponse>> GetItemsByCategoryAsync(int categoryId)
        {
            var items = await _foodRepository.GetItemsByCategoryIdAsync(categoryId);
            return items.Select(MapItemToResponse);
        }

        public async Task<FoodItemResponse?> GetItemByIdAsync(int id)
        {
            var item = await _foodRepository.GetItemByIdAsync(id);
            if (item == null) return null;
            return MapItemToResponse(item);
        }

        public async Task<FoodItemResponse?> CreateItemAsync(FoodItemRequest request)
        {
            var category = await _foodRepository.GetCategoryByIdAsync(request.CategoryId);
            if (category == null) return null;

            var item = new FoodItem
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                BasePrice = request.BasePrice,
                Calories = request.Calories,
                IsCombo = request.IsCombo,
                IsAvailable = request.IsAvailable,
                DisplayOrder = request.DisplayOrder,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _foodRepository.AddItemAsync(item);
            await _foodRepository.SaveChangesAsync();
            return await GetItemByIdAsync(item.Id);
        }

        public async Task<FoodItemResponse?> UpdateItemAsync(int id, FoodItemRequest request)
        {
            var item = await _foodRepository.GetItemByIdAsync(id);
            if (item == null) return null;

            var category = await _foodRepository.GetCategoryByIdAsync(request.CategoryId);
            if (category == null) return null;

            item.CategoryId = request.CategoryId;
            item.Name = request.Name;
            item.Description = request.Description;
            item.ImageUrl = request.ImageUrl;
            item.BasePrice = request.BasePrice;
            item.Calories = request.Calories;
            item.IsCombo = request.IsCombo;
            item.IsAvailable = request.IsAvailable;
            item.DisplayOrder = request.DisplayOrder;
            item.UpdatedAt = DateTime.Now;

            _foodRepository.UpdateItem(item);
            await _foodRepository.SaveChangesAsync();
            return await GetItemByIdAsync(id);
        }

        public async Task<IEnumerable<FoodComboItemResponse>> GetComboDetailAsync(int comboId)
        {
            var comboItems = await _foodRepository.GetComboItemsAsync(comboId);
            return comboItems.Select(c => new FoodComboItemResponse
            {
                ItemId = c.ItemId,
                ItemName = c.Item?.Name ?? "N/A",
                Quantity = c.Quantity
            });
        }

        public async Task<bool> UpdateComboDetailAsync(int comboId, List<FoodComboItemRequest> requests)
        {
            var combo = await _foodRepository.GetItemByIdAsync(comboId);
            if (combo == null || !combo.IsCombo) return false;

            var newComboItems = requests.Select(r => new FoodComboItem
            {
                ComboId = comboId,
                ItemId = r.ItemId,
                Quantity = r.Quantity
            }).ToList();

            await _foodRepository.UpdateComboItemsAsync(comboId, newComboItems);
            await _foodRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var item = await _foodRepository.GetItemByIdAsync(id);
            if (item == null) return false;

            item.IsAvailable = false; // Soft Delete
            item.UpdatedAt = DateTime.Now;
            _foodRepository.UpdateItem(item);
            await _foodRepository.SaveChangesAsync();
            return true;
        }
    }
}