using EventBookingService.Models;
using EventBookingService.Repositories;
using EventBookingService.Exceptions;
using EventBookingService.DTOs;

namespace EventBookingService.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public Event GetEventById(Guid id) => 
        _eventRepository.GetById(id) 
        ?? throw new NotFoundException($"Мероприятие с id {id} не найдено");

        public IReadOnlyList<Event> GetEvents(EventQuery eventQuery)
        {
            if (eventQuery.From.HasValue &&
                eventQuery.To.HasValue &&
                eventQuery.From > eventQuery.To)
                throw new ValidationException("Дата From не может быть позже даты To.");
                
            return _eventRepository.GetEvents(eventQuery);
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

        public Event UpdateEvent(Guid id, string title, string? description, DateTime startAt, DateTime endAt)
        {
            ValidateDates(startAt, endAt);
            var existingEvent = GetEventById(id);
            existingEvent.Title = title;
            existingEvent.Description = description;
            existingEvent.StartAt = startAt;
            existingEvent.EndAt = endAt;
            _eventRepository.Update(existingEvent);
            return existingEvent;
        }

        public void DeleteEventById(Guid id)
        {
            var isDeleted =_eventRepository.Delete(id);
            if(!isDeleted) throw new NotFoundException($"Мероприятие с id {id} не найдено");
        }

        private void ValidateDates(DateTime startAt, DateTime endAt)
        {
            if (endAt <= startAt)
            {
                throw new ValidationException("Дата окончания должна быть позже даты начала.");
            }
        }
    }
}
