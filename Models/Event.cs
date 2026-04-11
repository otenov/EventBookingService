using System.Data;

namespace EventBookingService.Models
{
    public class Event
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

    }
}