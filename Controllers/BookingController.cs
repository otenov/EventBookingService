using EventBookingService.DTOs;
using EventBookingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingService.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingController(IBookingService bookingService) : ControllerBase
{
    /// <summary>
    /// Возвращает бронирование по id
    /// </summary>
    /// <param name="bookingId">Id бронирования</param>
    /// <response code="200">Бронирование найдено</response>
    /// <response code="404">Бронирование не найдено</response>
    [ProducesResponseType(typeof(BookingInfoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    [HttpGet("{bookingId}")]
    public async Task<IActionResult> GetBookingByIdAsync(Guid bookingId)
    {
        var bookingInfoDTO = await bookingService.GetBookingByIdAsync(bookingId);
        return Ok(bookingInfoDTO);
    }

    /// <summary>
    /// Создаёт бронирование под мероприятие с указанным id в состоянии pending
    /// </summary>
    /// <param name="eventId">Id мероприятия</param>
    /// <response code = "202">Бронирование в обработке</response>
    /// <response code = "404">Мероприятие не найдено</response>
    [ProducesResponseType(typeof(BookingInfoDTO),StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    [HttpPost("/api/events/{eventId}/book")]
    public async Task<IActionResult> CreateBookingAsync(Guid eventId)
    {
        var booking = await bookingService.CreateBookingAsync(eventId);
        string location = $"/api/bookings/{booking.Id}";
        return Accepted(location,booking);
    }
}