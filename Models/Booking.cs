
namespace EventBookingService.Models;

public class Booking()
{
    public Guid Id {get; init;} = Guid.NewGuid();

    public Guid EventId {get; init;}

    public BookingStatus Status {get; set;} = BookingStatus.Pending;

    public DateTime CreatedAt {get; set;} = DateTime.Now;

    public DateTime? ProcesseAt{get;set;}
}