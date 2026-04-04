using CinemaWebApi.DTOs.Responses;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<MembershipCardResponse> GetMyCardAsync(Guid userId);
        Task EarnPointsAsync(Guid userId, Guid bookingId, decimal finalAmount);
    }
}
