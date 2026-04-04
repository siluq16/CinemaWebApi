using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionResponse>> GetAllPromotionsAsync();
        Task<PromotionResponse?> GetPromotionByIdAsync(Guid id);

        Task<PromotionResponse> CreatePromotionAsync(CreatePromotionRequest request);

        Task<PromotionResponse?> UpdatePromotionAsync(Guid id, CreatePromotionRequest request);
        Task<bool> DeletePromotionAsync(Guid id);
    }
}