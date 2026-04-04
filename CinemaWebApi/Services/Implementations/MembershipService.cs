using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepo;

        public MembershipService(IMembershipRepository membershipRepo)
        {
            _membershipRepo = membershipRepo;
        }

        public async Task<MembershipCardResponse> GetMyCardAsync(Guid userId)
        {
            var card = await _membershipRepo.GetByUserIdAsync(userId);

            if (card == null)
            {
                card = new MembershipCard
                {
                    UserId = userId,
                    CardNumber = GenerateCardNumber(),
                    Tier = "silver",
                    TotalPoints = 0,
                    UsedPoints = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _membershipRepo.CreateCardAsync(card);
                await _membershipRepo.SaveChangesAsync();
            }

            return new MembershipCardResponse
            {
                CardNumber = card.CardNumber,
                Tier = card.Tier,
                TotalPoints = card.TotalPoints,
                UsedPoints = card.UsedPoints
            };
        }

        public async Task EarnPointsAsync(Guid userId, Guid bookingId, decimal finalAmount)
        {
            var card = await _membershipRepo.GetByUserIdAsync(userId);
            if (card == null)
            {
                await GetMyCardAsync(userId);
                card = await _membershipRepo.GetByUserIdAsync(userId);
            }

            int earnedPoints = (int)(finalAmount * 0.05m);

            if (earnedPoints > 0)
            {
                card!.TotalPoints += earnedPoints;
                card.UpdatedAt = DateTime.Now;

                var transaction = new PointTransaction
                {
                    MembershipId = card.Id,
                    BookingId = bookingId,
                    Points = earnedPoints,
                    Reason = $"Tích lũy 5% từ hóa đơn mua vé",
                    CreatedAt = DateTime.Now
                };

                await _membershipRepo.AddTransactionAsync(transaction);

                // (Nâng cao) Logic thăng hạng: Nếu TotalPoints > 2.000.000 thì lên hạng Gold...
                if (card.TotalPoints >= 2000000 && card.Tier == "silver") card.Tier = "gold";
                else if (card.TotalPoints >= 5000000 && card.Tier == "gold") card.Tier = "platinum";

                await _membershipRepo.SaveChangesAsync();
            }
        }

        private string GenerateCardNumber()
        {
            var random = new Random();
            return "MEM" + random.Next(10000000, 99999999).ToString(); // VD: MEM12345678
        }
    }
}
