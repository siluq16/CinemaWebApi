using CinemaWebApi.DTOs.Requests;
using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")] // CHỈ admin ĐƯỢC PHÉP TRUY CẬP
    public class PricingsController : ControllerBase
    {
        private readonly IPricingService _pricingService;

        public PricingsController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        // ==========================
        // API LUẬT GIÁ (PRICING RULES)
        // ==========================
        [HttpGet("rules")]
        public async Task<IActionResult> GetAllRules()
        {
            return Ok(await _pricingService.GetAllRulesAsync());
        }

        [HttpPost("rules")]
        public async Task<IActionResult> CreateRule([FromBody] PricingRuleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _pricingService.CreateRuleAsync(request);
            return Ok(result);
        }

        [HttpPut("rules/{id:int}")]
        public async Task<IActionResult> UpdateRule(int id, [FromBody] PricingRuleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _pricingService.UpdateRuleAsync(id, request);
            if (result == null) return NotFound("Không tìm thấy luật giá.");
            return Ok(result);
        }

        [HttpPatch("rules/{id:int}/toggle")]
        public async Task<IActionResult> ToggleRuleStatus(int id)
        {
            var success = await _pricingService.ToggleRuleStatusAsync(id);
            if (!success) return NotFound("Không tìm thấy luật giá.");
            return Ok(new { message = "Đã thay đổi trạng thái luật giá." });
        }

        // ==========================
        // API NGÀY LỄ (HOLIDAYS)
        // ==========================
        [HttpGet("holidays")]
        public async Task<IActionResult> GetAllHolidays()
        {
            return Ok(await _pricingService.GetAllHolidaysAsync());
        }

        [HttpPost("holidays")]
        public async Task<IActionResult> CreateHoliday([FromBody] HolidayRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _pricingService.CreateHolidayAsync(request);
            return Ok(result);
        }

        [HttpPut("holidays/{id:int}")]
        public async Task<IActionResult> UpdateHoliday(int id, [FromBody] HolidayRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _pricingService.UpdateHolidayAsync(id, request);
            if (result == null) return NotFound("Không tìm thấy ngày lễ.");
            return Ok(result);
        }

        [HttpDelete("holidays/{id:int}")]
        public async Task<IActionResult> DeleteHoliday(int id)
        {
            var success = await _pricingService.DeleteHolidayAsync(id);
            if (!success) return NotFound("Không tìm thấy ngày lễ.");
            return Ok(new { message = "Đã xóa ngày lễ." });
        }
    }
}