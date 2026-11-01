using EventBookingService.DTOs;

namespace EventBookingService.Services;

public interface IBookingService
{
    Task<BookingInfoDTO> CreateBookingAsync(Guid eventId);

    Task<BookingInfoDTO> GetBookingByIdAsync(Guid bookingID);
}