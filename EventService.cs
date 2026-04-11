using EventBookingService.Models;

namespace EventBookingService
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository) //TODO: зарегать сервис в DI
        {
            _eventRepository=eventRepository;
        }

        public Event? GetEventById(Guid id)
        {
            return _eventRepository.GetById(id);
        }

        public IReadOnlyList<Event> GetEvents()
        {
            return _eventRepository.GetEvents();
        }

        public Event CreateEvent(string title, string? description, DateTime startAt, DateTime endAt)
        {
            Event @event = new Event()
            {
                Title = title,
                Description = description,
                StartAt = startAt,
                EndAt = endAt
            };
            _eventRepository.Save(@event);
            return @event;
        }

        public Event? UpdateEvent(Event @event)
        {
            var updatedEvent = _eventRepository.Update(@event);
            return updatedEvent;
        }

        public bool DeleteEventById(Guid id)
        {
            return _eventRepository.DeleteById(id);
        }
    }
}
