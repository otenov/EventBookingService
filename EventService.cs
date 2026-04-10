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

        public Event GetEventById(Guid id)
        {
            //авторизация
            return _eventRepository.GetById(id);
            //проверяю авторизацию
            //вызываю логгер
            //
        }

        public IReadOnlyList<Event> GetEvents()
        {
            return _eventRepository.GetEvents();
        }

        public Event CreateEvent(string title, string description, DateTime startAt, DateTime endAt)
        {
            Event _event = new Event()
            {
                Title = title,
                Description = description,
                StartAt = startAt,
                EndAt = endAt
            };
            _eventRepository.Save(_event);
            return _event;
        }
        public Event UpdateEvent(Event @event)
        {
            var _event = _eventRepository.Update(@event);
            return _event;
        }

        public Guid DeleteEventById(Guid id)
        {
            var _id = _eventRepository.DeleteById(id);
            return id;
        }
    }
}
