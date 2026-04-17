using System.ComponentModel.DataAnnotations;

namespace EventBookingService
{
    public class UpdateEventDTO
    {
        [Required(ErrorMessage = "Заголовок обязателен для заполнения")]
        [StringLength(50, ErrorMessage = "Название события не должно превышать 50 символов")]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Дата старта события обязательная для заполнения")]
        [Range(typeof(DateTime), "01.01.2000", "01.01.3000", ErrorMessage = "Некорретная дата")]
        public DateTime StartAt { get; set; }

        [Required(ErrorMessage = "Дата завершения события обязательная для заполнения")]
        [Range(typeof(DateTime), "01.01.2000", "01.01.3000", ErrorMessage = "Некорретная дата")]
        public DateTime EndAt { get; set; }
    }
}
