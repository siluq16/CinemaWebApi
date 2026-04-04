using CinemaWebApi.DTOs.Requests;

namespace CinemaWebApi.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(ProcessPaymentRequest request, Guid userId);
        Task<string> CreateVnPayPaymentUrlAsync(Guid bookingId, HttpContext context);
        Task<string> ProcessVnPayIpnAsync(IQueryCollection queryData);
    }
}