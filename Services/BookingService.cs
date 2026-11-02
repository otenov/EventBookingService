using EventBookingService.DTOs;
using EventBookingService.Exceptions;
using EventBookingService.Models;
using EventBookingService.Repositories;

namespace EventBookingService.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventRepository _eventRepository;

    private readonly IBookingTaskQueue _bookingTaskQueue;

    public BookingService(IBookingRepository bookingRepository, IEventRepository eventRepository, IBookingTaskQueue bookingTaskQueue)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _bookingTaskQueue = bookingTaskQueue;
    }

    public Task<BookingInfoDTO> CreateBookingAsync(Guid eventId)
    {
        if(_eventRepository.GetById(eventId) is null)
            throw new NotFoundException($"Мероприятие с id {eventId} не найдено");
        var booking = new Booking
        {
            EventId = eventId,
        };
        _bookingRepository.Save(booking);
        _bookingTaskQueue.Enqueue(booking);
        var bookingInfoDTO = ToDto(booking);
        return Task.FromResult(bookingInfoDTO);
    }

    public Task<BookingInfoDTO> GetBookingByIdAsync(Guid bookingID)
    {
        var booking = _bookingRepository.GetById(bookingID);
        if (booking is null)
            throw new NotFoundException($"Бронирование с id {bookingID} не найдено");
        var bookingInfoDTO = ToDto(booking);
        return Task.FromResult(bookingInfoDTO);
    }

    private BookingInfoDTO ToDto(Booking booking)
    {
        var bookingInfoDTO = new BookingInfoDTO
        {
            Id = booking.Id,
            EventId = booking.EventId,
            CreatedAt = booking.CreatedAt,
            Status = booking.Status,
            ProcessedAt = booking.ProcessedAt
        };
        return bookingInfoDTO;
    }
}