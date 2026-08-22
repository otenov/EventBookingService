using EventBookingService.Models;

namespace EventBookingService.Repositories
{
    public interface IEventRepository
    {

        Event? GetById(Guid id);

        IReadOnlyList<Event> GetEvents();

        void Save(Event @event);

        void Delete(Event @event);

    }
}
