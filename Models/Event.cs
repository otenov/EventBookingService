using System.Data;

namespace EventBookingService.Models
{
    public class Event
    {
        public required Guid Id { get; init; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        public required DateTime StartAt { get; set; }

        public required DateTime EndAt { get; set; }

        public Event(string title, string description, DateTime startAt, DateTime endAt)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            StartAt = startAt;
            EndAt = endAt;
        }

    }
}