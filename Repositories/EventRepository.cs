using EventBookingService.DTOs;
using EventBookingService.Models;
namespace EventBookingService.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly List<Event> _events = [];

        public Event? GetById(Guid id)
        {
            return _events.FirstOrDefault(e => e.Id == id);
        }

        public PagedData GetEvents(EventQuery eventQuery)
        {
            IEnumerable<Event> query = _events;

            if(!string.IsNullOrWhiteSpace(eventQuery.Title))
            query = query.Where(e=>e.Title.Contains(eventQuery.Title, StringComparison.OrdinalIgnoreCase));

            if(eventQuery.From is DateTime from)
            query = query.Where(e=>e.StartAt>=from);

            if(eventQuery.To is DateTime to)
            query = query.Where(e=>e.EndAt<=to);

            var totalCount = query.Count();

            var events = query.Skip((eventQuery.Page - 1)*eventQuery.PageSize)
                           .Take(eventQuery.PageSize)
                           .ToList();

            return new PagedData
            {
              Events = events,
              TotalCount = totalCount  
            };
        }

        public void Save(Event @event)
        {
            _events.Add(@event);
        }

        public void Update(Event @event)
        {
            // In-memory: объект уже изменён по ссылке.
        }

        public bool Delete(Guid id)
        {
            var existingEvent = GetById(id);
            if (existingEvent is null) return false;
            _events.Remove(existingEvent);
            return true;
        }
    }
}
