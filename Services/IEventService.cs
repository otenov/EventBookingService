using EventBookingService.DTOs;
using EventBookingService.Models;
namespace EventBookingService.Services
{
    public interface IEventService
    {
        Event GetEventById(Guid id);

        IReadOnlyList<Event> GetEvents(EventQuery eventQuery);

        Event CreateEvent(string title, string? description, DateTime startAt, DateTime endAt);

        Event UpdateEvent(Guid id, string title, string? description, DateTime startAt, DateTime endAt);

        void DeleteEventById(Guid id);



    }
}
