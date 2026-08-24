using EventBookingService.Models;
using EventBookingService.Services;

namespace EventBookingService.Repositories
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

        public void Uodate(Event @event)
        {
            // In-memory: объект уже изменён по ссылке.
        }

        public bool Delete(Guid id)
        {
            var existingEvent = GetById(id);
            if(existingEvent is null) return false;
            _events.Remove(existingEvent);
            return true;
        }
    }
}
