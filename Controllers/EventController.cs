using Microsoft.AspNetCore.Mvc;

namespace EventBookingService.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            return Ok(_eventService.GetEvents());
        }

        [HttpGet("{id}")]
        public IActionResult GetEventById(Guid id)
        {
            var @event = _eventService.GetEventById(id);
            if (@event is null) return NotFound();
            return Ok(@event);
        }

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

        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(Guid id)
        {
            var deleted = _eventService.DeleteEventById(id);
            if (!deleted) return NotFound();
            return NoContent(); 
        }
    }
}
