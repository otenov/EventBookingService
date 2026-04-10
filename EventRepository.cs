using EventBookingService.Models;
using System.Reflection.Metadata.Ecma335;

namespace EventBookingService
{
    public class EventRepository : IEventRepository
    {
        private readonly List<Event> events;

        public Event GetById(Guid id)
        {
            var _event = events.FirstOrDefault(e => e.Id == id);
            return _event;
        }

        public IReadOnlyList<Event> GetEvents()
        {
            return events;
        }

        public void Save(Event @event)
        {
            events.Add(@event);
        }

        public Event Update(Event @event)
        {
            var _event = GetById(@event.Id);
            _event.Title = @event.Title;
            _event.Description = @event.Description;
            _event.StartAt = @event.StartAt;
            _event.EndAt = @event.EndAt;
            return _event;
        }

        public Guid DeleteById(Guid id)
        {
            var _event = GetById(id);
            events.Remove(_event);
            return id;
        }
    }
}
