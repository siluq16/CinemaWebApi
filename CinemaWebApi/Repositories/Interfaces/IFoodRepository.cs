using CinemaWebApi.Models;

namespace CinemaWebApi.Repositories.Interfaces
{
    public interface IFoodRepository
    {
        Task<IEnumerable<FoodCategory>> GetAllCategoriesAsync();
        Task<FoodCategory?> GetCategoryByIdAsync(int id);
        Task<FoodCategory> AddCategoryAsync(FoodCategory category);
        void UpdateCategory(FoodCategory category);

        Task<IEnumerable<FoodItem>> GetAllItemsAsync();
        Task<IEnumerable<FoodItem>> GetItemsByCategoryIdAsync(int categoryId);
        Task<FoodItem?> GetItemByIdAsync(int id);
        Task<FoodItem> AddItemAsync(FoodItem item);
        void UpdateItem(FoodItem item);

        Task<IEnumerable<FoodComboItem>> GetComboItemsAsync(int comboId);
        Task UpdateComboItemsAsync(int comboId, IEnumerable<FoodComboItem> items);

        Task<bool> SaveChangesAsync();
    }
}