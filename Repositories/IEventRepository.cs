using EventBookingService.DTOs;
using EventBookingService.Models;

namespace EventBookingService.Repositories
{
    public interface IEventRepository
    {

        Event? GetById(Guid id);

        PagedData GetEvents(EventQuery eventQuery);

        void Save(Event @event);

        void Update(Event @event);

        bool Delete(Guid id);

    }
}
