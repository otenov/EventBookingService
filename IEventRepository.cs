using EventBookingService.Models;
using System.Xml.Linq;

namespace EventBookingService
{
    public interface IEventRepository
    {

        Event? GetById(Guid id);

        IReadOnlyList<Event> GetEvents();

        void Save(Event @event);

        void Delete(Event @event);

    }
}
