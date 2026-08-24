using EventBookingService.Models;
using EventBookingService.DTOs;
using EventBookingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventBookingService.Controllers
{
    /// <summary>
    /// Контроллер для управления мероприятиями.
    /// </summary>
    [Route("api/events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;


        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        /// <summary>
        /// Возвращает все мероприятия
        /// </summary>
        /// <response code="200">Мероприятия найдены</response>
        [ProducesResponseType(typeof(IReadOnlyList<Event>),StatusCodes.Status200OK)]
        [Produces("application/json")]
        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(_eventService.GetEvents());
        }

        /// <summary>
        /// Возвращает мероприятие по id
        /// </summary>
        /// <param name="id">Id мероприятия</param>
        /// <response code="200">Мероприятие найдено </response>
        /// <response code="404">Мероприятие не найдено </response>
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [HttpGet("{id}")]
        public IActionResult GetEventById(Guid id)
        {
            var @event = _eventService.GetEventById(id);
            if (@event is null) return NotFound();
            return Ok(@event);
        }

        /// <summary>
        /// Создаёт новое мероприятие
        /// </summary>
        /// <param name="createEventDTO">Данные нового мероприятия</param>
        /// <response code="201">Мероприятие успешно создано</response>
        /// <response code="400">Переданы некорректные данные</response>
        [ProducesResponseType(typeof(Event), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPost]
        public IActionResult CreateEvent([FromBody] CreateEventDTO createEventDTO)
        {
            try
            {
                var createdEvent = _eventService.CreateEvent(
                createEventDTO.Title,
                createEventDTO.Description,
                createEventDTO.StartAt,
                createEventDTO.EndAt);
                return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {error = ex.Message});
            }
           
        }

        /// <summary>
        /// Обновляет данные мероприятия по id
        /// </summary>
        /// <param name="id">Id мероприятия</param>
        /// <param name="updateEventDTO">Новые данные мероприятия</param>
        /// <response code="200">Мероприятие успешно обновлено</response>
        /// <response code="404">Мероприятие не найдено</response>
        /// <response code="400">Переданы некорректные данные</response>
        [ProducesResponseType(typeof(Event), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [Consumes("application/json")]
        [HttpPut("{id}")]
        public IActionResult UpdateEvent(Guid id, [FromBody] UpdateEventDTO updateEventDTO)
        {
            try
            {
                var updatedEvent = _eventService.UpdateEvent(
                id, 
                updateEventDTO.Title, 
                updateEventDTO.Description, 
                updateEventDTO.StartAt, 
                updateEventDTO.EndAt);
                if(updatedEvent is null) return NotFound();
                return Ok(updatedEvent);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {error = ex.Message});
            }

        }
        /// <summary>
        /// Удаление мероприятие по id
        /// </summary>
        /// <param name="id">Id мероприятия</param>
        /// <response code="204">Мероприятие успешно удалено</response>
        /// <response code="404">Мероприятие не найдено</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(Guid id)
        {
            var deleted = _eventService.DeleteEventById(id);
            if (!deleted) return NotFound();
            return NoContent(); 
        }
    }
}
