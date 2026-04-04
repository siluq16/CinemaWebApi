using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")] 
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPromotions()
        {
            return Ok(await _promotionService.GetAllPromotionsAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPromotionById(Guid id)
        {
            var promotion = await _promotionService.GetPromotionByIdAsync(id);
            if (promotion == null) return NotFound(new { message = "Không tìm thấy mã khuyến mãi." });

            return Ok(promotion);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var response = await _promotionService.CreatePromotionAsync(request);
                return CreatedAtAction(nameof(GetPromotionById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePromotion(Guid id, [FromBody] CreatePromotionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var response = await _promotionService.UpdatePromotionAsync(id, request);
                if (response == null) return NotFound(new { message = "Không tìm thấy mã khuyến mãi để cập nhật." });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePromotion(Guid id)
        {
            // API này gọi là Delete nhưng thực chất là thao tác "Vô hiệu hóa" (Khóa mã)
            var isDeleted = await _promotionService.DeletePromotionAsync(id);
            if (!isDeleted) return NotFound(new { message = "Không tìm thấy mã khuyến mãi." });

            return Ok(new { message = "Đã vô hiệu hóa mã khuyến mãi thành công." });
        }
    }
}