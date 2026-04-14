using CinemaWebApi.DTOs.Responses;
using CinemaWebApi.Models;
using CinemaWebApi.Repositories.Interfaces;
using CinemaWebApi.Services.Interfaces;

namespace CinemaWebApi.Services.Implementations
{
    public class MembershipService : IMembershipService
    {
        private readonly IMembershipRepository _membershipRepo;
        private readonly INotificationService _notiService;

        public MembershipService(IMembershipRepository membershipRepo, INotificationService notiService)
        {
            _membershipRepo = membershipRepo;
            _notiService = notiService;
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

            int earnedPoints = (int)(finalAmount / 10000);

            if (earnedPoints > 0)
            {
                card!.TotalPoints += earnedPoints;
                card.UpdatedAt = DateTime.Now;

                var transaction = new PointTransaction
                {
                    MembershipId = card.Id,
                    BookingId = bookingId,
                    Points = earnedPoints,
                    Reason = $"Tích lũy từ hóa đơn mua vé",
                    CreatedAt = DateTime.Now
                };

                await _membershipRepo.AddTransactionAsync(transaction);
                string newTier = card.Tier;
                if (card.TotalPoints >= 2000 && card.Tier == "silver")
                {
                    newTier = "gold";
                }
                else if (card.TotalPoints >= 5000 && card.Tier == "gold")
                {
                    newTier = "platinum";
                }

                if (newTier != card.Tier)
                {
                    card.Tier = newTier;

                    await _notiService.SendNotificationAsync(
                        userId,
                        "🎉 Thăng hạng thành viên!",
                        $"Chúc mừng! Bạn đã được thăng hạng lên thành viên {newTier.ToUpper()} và sẽ nhận được nhiều ưu đãi mới.",
                        "tier_upgrade"
                    );
                }

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
