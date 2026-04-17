using EventBookingService.Models;
namespace EventBookingService
{
    public interface IEventService
    {
        Event? GetEventById(Guid id);

        IReadOnlyList<Event> GetEvents();

        Event CreateEvent(string title, string? description, DateTime startAt, DateTime endAt);

        Event? UpdateEvent(Guid id, string title, string? description, DateTime startAt, DateTime endAt);

        bool DeleteEventById(Guid id);



    }
}
