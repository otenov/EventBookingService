using EventBookingService.Models;
using System.Reflection.Metadata.Ecma335;

namespace EventBookingService
{
    public class EventRepository : IEventRepository
    {
        private readonly List<Event> _events = [];

        public Event? GetById(Guid id)
        {
            var @event = _events.FirstOrDefault(e => e.Id == id);
            return @event;
        }

        public IReadOnlyList<Event> GetEvents()
        {
            return _events;
        }

        public void Save(Event @event)
        {
            _events.Add(@event);
        }

        public Event? Update(Event @event)
        {
            var existingEvent = GetById(@event.Id);
            if (existingEvent is null) return null;
            
            existingEvent.Title = @event.Title;
            existingEvent.Description = @event.Description;
            existingEvent.StartAt = @event.StartAt;
            existingEvent.EndAt = @event.EndAt;
            return existingEvent;
        }

        public bool DeleteById(Guid id)
        {
            var existingEvent = GetById(id);
            if (existingEvent is not null)
            {
                _events.Remove(existingEvent);
                return true;
            }
            return false;
        }
    }
}
