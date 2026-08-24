using System.ComponentModel.DataAnnotations;

namespace EventBookingService.DTOs
{
    public class UpdateEventDTO
    {
        [Required(ErrorMessage = "Заголовок обязателен для заполнения")]
        [StringLength(50, ErrorMessage = "Название события не должно превышать 50 символов")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Range(
            typeof(DateTime),
            "2000-01-01T00:00:00",
            "3000-01-01T00:00:00",
            ParseLimitsInInvariantCulture =true,
            ErrorMessage ="Некорректная дата")]
        public DateTime StartAt { get; set; }

        [Range(
            typeof(DateTime),
            "2000-01-01T00:00:00",
            "3000-01-01T00:00:00",
            ParseLimitsInInvariantCulture =true,
            ErrorMessage ="Некорректная дата")]
        public DateTime EndAt { get; set; }
    }
}
