using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepo;

        public PromotionService(IPromotionRepository promotionRepo)
        {
            _promotionRepo = promotionRepo;
        }

        private PromotionResponse MapToResponse(Promotion p)
        {
            return new PromotionResponse
            {
                Id = p.Id,
                Code = p.Code,
                Title = p.Title,
                Description = p.Description,
                DiscountType = p.DiscountType,
                DiscountValue = p.DiscountValue,
                MaxDiscountAmount = p.MaxDiscountAmount,
                MinOrderValue = p.MinOrderValue,
                MaxUses = p.MaxUses,
                MaxUsesPerUser = p.MaxUsesPerUser,
                ValidFrom = p.ValidFrom,
                ValidTo = p.ValidTo,
                IsActive = p.IsActive
            };
        }

        public async Task<IEnumerable<PromotionResponse>> GetAllPromotionsAsync()
        {
            var promotions = await _promotionRepo.GetAllAsync();
            return promotions.Select(MapToResponse);
        }

        // 2. READ ONE
        public async Task<PromotionResponse?> GetPromotionByIdAsync(Guid id)
        {
            var promotion = await _promotionRepo.GetByIdAsync(id);
            if (promotion == null) return null;
            return MapToResponse(promotion);
        }

        // 3. CREATE (Hàm cũ của bạn, mình tóm gọn lại phần return nhờ hàm MapToResponse)
        public async Task<PromotionResponse> CreatePromotionAsync(CreatePromotionRequest request)
        {
            if (request.ValidFrom >= request.ValidTo)
                throw new Exception("Ngày kết thúc phải lớn hơn ngày bắt đầu.");

            if (request.DiscountType == "percentage" && request.DiscountValue > 100)
                throw new Exception("Giảm giá theo phần trăm không được vượt quá 100%.");

            string normalizedCode = request.Code.Trim().ToUpper();
            bool exists = await _promotionRepo.CodeExistsAsync(normalizedCode);
            if (exists)
                throw new Exception($"Mã khuyến mãi '{normalizedCode}' đã tồn tại.");

            var promotion = new Promotion
            {
                Code = normalizedCode,
                Title = request.Title,
                Description = request.Description,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                MinOrderValue = request.MinOrderValue,
                AppliesToFood = request.AppliesToFood,
                MaxUses = request.MaxUses,
                MaxUsesPerUser = request.MaxUsesPerUser,
                UsedCount = 0,
                ValidFrom = request.ValidFrom,
                ValidTo = request.ValidTo,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            await _promotionRepo.AddAsync(promotion);
            await _promotionRepo.SaveChangesAsync();

            return MapToResponse(promotion);
        }

        // 4. UPDATE
        public async Task<PromotionResponse?> UpdatePromotionAsync(Guid id, CreatePromotionRequest request)
        {
            var promotion = await _promotionRepo.GetByIdAsync(id);
            if (promotion == null) return null;

            if (request.ValidFrom >= request.ValidTo)
                throw new Exception("Ngày kết thúc phải lớn hơn ngày bắt đầu.");

            string normalizedCode = request.Code.Trim().ToUpper();

            // Check xem admin có sửa mã Code thành một mã đã tồn tại của Khuyến mãi KHÁC hay không
            bool isCodeDuplicate = await _promotionRepo.CodeExistsExceptIdAsync(normalizedCode, id);
            if (isCodeDuplicate)
                throw new Exception($"Mã khuyến mãi '{normalizedCode}' đã được sử dụng cho một chương trình khác.");

            // Cập nhật các trường
            promotion.Code = normalizedCode;
            promotion.Title = request.Title;
            promotion.Description = request.Description;
            promotion.DiscountType = request.DiscountType;
            promotion.DiscountValue = request.DiscountValue;
            promotion.MaxDiscountAmount = request.MaxDiscountAmount;
            promotion.MinOrderValue = request.MinOrderValue;
            promotion.AppliesToFood = request.AppliesToFood;
            promotion.MaxUses = request.MaxUses;
            promotion.MaxUsesPerUser = request.MaxUsesPerUser;
            promotion.ValidFrom = request.ValidFrom;
            promotion.ValidTo = request.ValidTo;

            _promotionRepo.Update(promotion);
            await _promotionRepo.SaveChangesAsync();

            return MapToResponse(promotion);
        }

        // 5. DELETE (XÓA MỀM)
        public async Task<bool> DeletePromotionAsync(Guid id)
        {
            var promotion = await _promotionRepo.GetByIdAsync(id);
            if (promotion == null) return false;

            // Xóa mềm: Chỉ chuyển trạng thái thành Inactive để khách không nhập được nữa
            // Chứ không xóa hẳn khỏi DB để tránh lỗi khóa ngoại với bảng Hóa đơn
            promotion.IsActive = false;

            _promotionRepo.Update(promotion);
            await _promotionRepo.SaveChangesAsync();

            return true;
        }
    }
}