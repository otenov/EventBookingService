using EventBookingService.Models;

namespace EventBookingService.Repositories
{
    public interface IEventRepository
    {

        Event? GetById(Guid id);

        IReadOnlyList<Event> GetEvents();

        void Save(Event @event);

        void Update(Event @event);

        bool Delete(Guid id);

    }
}
