using EventBookingService.Models;
namespace EventBookingService
{
    public interface IEventService
    {
        Event CreateEvent(string title, string description, DateTime startAt, DateTime endAt);

        void DeleteEvent(int id);

        List<Event> GetEvents();

        Event GetEventById(int id);

        void UpdateEvent(int id); //Как обновлять?


    }
}
