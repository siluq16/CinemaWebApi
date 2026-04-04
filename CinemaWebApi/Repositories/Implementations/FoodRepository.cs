using CinemaWebApi.Data;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebApi.Repositories.Implementations
{
    public class FoodRepository : IFoodRepository
    {
        private readonly CinemaWebApiContext _context;

        public FoodRepository(CinemaWebApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FoodCategory>> GetAllCategoriesAsync()
        {
            return await _context.FoodCategories.OrderBy(c => c.DisplayOrder).ToListAsync();
        }

        public async Task<FoodCategory?> GetCategoryByIdAsync(int id)
        {
            return await _context.FoodCategories.FindAsync(id);
        }

        public async Task<FoodCategory> AddCategoryAsync(FoodCategory category)
        {
            await _context.FoodCategories.AddAsync(category);
            return category;
        }

        public void UpdateCategory(FoodCategory category)
        {
            _context.FoodCategories.Update(category);
        }

        public async Task<IEnumerable<FoodItem>> GetAllItemsAsync()
        {
            return await _context.FoodItems
                .Include(f => f.Category) // JOIN lấy tên danh mục
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<FoodItem>> GetItemsByCategoryIdAsync(int categoryId)
        {
            return await _context.FoodItems
                .Include(f => f.Category)
                .Where(f => f.CategoryId == categoryId)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync();
        }

        public async Task<FoodItem?> GetItemByIdAsync(int id)
        {
            return await _context.FoodItems
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FoodItem> AddItemAsync(FoodItem item)
        {
            await _context.FoodItems.AddAsync(item);
            return item;
        }

        public void UpdateItem(FoodItem item)
        {
            _context.FoodItems.Update(item);
        }

        public async Task<IEnumerable<FoodComboItem>> GetComboItemsAsync(int comboId)
        {
            return await _context.FoodComboItems
                .Include(fc => fc.Item) 
                .Where(fc => fc.ComboId == comboId)
                .ToListAsync();
        }

        public async Task UpdateComboItemsAsync(int comboId, IEnumerable<FoodComboItem> items)
        {
            var oldItems = await _context.FoodComboItems.Where(fc => fc.ComboId == comboId).ToListAsync();
            _context.FoodComboItems.RemoveRange(oldItems);

            await _context.FoodComboItems.AddRangeAsync(items);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}