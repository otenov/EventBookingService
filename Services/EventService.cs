using EventBookingService.Models;
using EventBookingService.Repositories;

namespace EventBookingService.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
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
            ValidateDates(startAt, endAt);
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

        public Event? UpdateEvent(Guid id, string title, string? description, DateTime startAt, DateTime endAt)
        {
            ValidateDates(startAt, endAt);
            var existingEvent = _eventRepository.GetById(id);
            if (existingEvent is null) return null;
            existingEvent.Title = title;
            existingEvent.Description = description;
            existingEvent.StartAt = startAt;
            existingEvent.EndAt = endAt;
            _eventRepository.Uodate(existingEvent);
            return existingEvent;
        }

        public bool DeleteEventById(Guid id)
        {
            return _eventRepository.Delete(id);
        }

        private void ValidateDates(DateTime startAt, DateTime endAt)
        {
            if (endAt <= startAt)
            {
                throw new ArgumentException("Дата окончания должна быть позже даты начала.");
            }
        }
    }
}
