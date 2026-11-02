using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using EventBookingService.Models;

namespace EventBookingService.Services;

public class InMemoryBookingTaskQueue : IBookingTaskQueue
{
    private ConcurrentQueue<Booking> _bookingQueue = new();
    public void Enqueue(Booking booking)
    {
       _bookingQueue.Enqueue(booking);
    }

    public bool TryDequeue([MaybeNullWhen(false)] out Booking booking)
    {
        return _bookingQueue.TryDequeue(out booking);
    }
}