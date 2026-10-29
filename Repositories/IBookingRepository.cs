using EventBookingService.Models;

namespace EventBookingService.Repositories;

public interface IBookingRepository
{
    Booking? GetById(Guid bookingId);

    void Save(Booking booking);

    void Update(Booking booking);

    bool Delete(Guid bookingId);


}