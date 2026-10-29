using EventBookingService.Models;

namespace EventBookingService.Repositories;

public class BookingRepositories() : IBookingRepository
{
    private readonly List<Booking> _bookings = [];

    public Booking? GetById(Guid bookingId)
    {
        return _bookings.FirstOrDefault(b=>b.Id == bookingId);
    }

    public void Save(Booking booking)
    {
        _bookings.Add(booking);
    }

    public void Update(Booking booking)
    {
        // In-memory: объект уже изменён по ссылке.
    }

    public bool Delete(Guid bookingId)
    {
        var existingBooking = GetById(bookingId);
        if (existingBooking is null) return false;
        _bookings.Remove(existingBooking);
        return true;
    }
}