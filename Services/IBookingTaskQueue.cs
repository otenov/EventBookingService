using System.Diagnostics.CodeAnalysis;
using EventBookingService.Models;

namespace EventBookingService.Services;

public interface IBookingTaskQueue
{
    void Enqueue(Booking booking);

    bool TryDequeue([MaybeNullWhen(false)] out Booking booking);
}