using CinemaWebApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaWebApi.Controllers
{
    [ApiController]
    [Route("api/cinemarevenue")]
    public class CinemaRevenueController : ControllerBase
    {
        private readonly ICinemaRevenueService _revenueService;

        public CinemaRevenueController(ICinemaRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCinemaRevenues()
        {
            try
            {
                var data = await _revenueService.GetCinemaRevenuesAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [HttpGet("monthly")] 
        public async Task<IActionResult> GetMonthlyRevenue([FromQuery] int year)
        {
            if (year <= 0) year = DateTime.Now.Year;
            return Ok(await _revenueService.GetMonthlyRevenueAsync(year));
        }

        [HttpGet("weekly")] 
        public async Task<IActionResult> GetWeeklyRevenue()
        {
            return Ok(await _revenueService.GetWeeklyRevenueAsync());
        }
    }
}
