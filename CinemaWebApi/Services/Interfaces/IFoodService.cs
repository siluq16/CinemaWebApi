using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IFoodService
    {
        // Category
        Task<IEnumerable<FoodCategoryResponse>> GetAllCategoriesAsync();
        Task<FoodCategoryResponse?> GetCategoryByIdAsync(int id);
        Task<FoodCategoryResponse> CreateCategoryAsync(FoodCategoryRequest request);
        Task<FoodCategoryResponse?> UpdateCategoryAsync(int id, FoodCategoryRequest request);

        // Item
        Task<IEnumerable<FoodItemResponse>> GetAllItemsAsync();
        Task<IEnumerable<FoodItemResponse>> GetItemsByCategoryAsync(int categoryId);
        Task<FoodItemResponse?> GetItemByIdAsync(int id);
        Task<FoodItemResponse?> CreateItemAsync(FoodItemRequest request);
        Task<FoodItemResponse?> UpdateItemAsync(int id, FoodItemRequest request);

        Task<IEnumerable<FoodComboItemResponse>> GetComboDetailAsync(int comboId);
        Task<bool> UpdateComboDetailAsync(int comboId, List<FoodComboItemRequest> requests);
        Task<bool> DeleteItemAsync(int id); // Soft delete (IsAvailable = false)
    }
}