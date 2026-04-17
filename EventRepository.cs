using EventBookingService.Models;
using System.Reflection.Metadata.Ecma335;

namespace EventBookingService
{
    public class EventRepository : IEventRepository
    {
        private readonly List<Event> _events = [];

        public Event? GetById(Guid id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }

        public IReadOnlyList<Event> GetEvents()
        {
            return _events;
        }

        public void Save(Event @event)
        {
            _events.Add(@event);
        }

        public void Delete(Event existingEvent)
        {
            _events.Remove(existingEvent);
        }
    }
}
