using EventBookingService.Models;
namespace EventBookingService
{
    public interface IEventService
    {
        Event? GetEventById(Guid id);

        IReadOnlyList<Event> GetEvents();

        Event CreateEvent(string title, string? description, DateTime startAt, DateTime endAt);

        Event? UpdateEvent(Event @event);

        bool DeleteEventById(Guid id);



    }
}
