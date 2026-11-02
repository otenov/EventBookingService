using EventBookingService.Models;

namespace EventBookingService.Services;

public class BookingBackgroundService(IBookingTaskQueue bookingTaskQueue, ILogger<BookingBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Сервис обработки задач на бронирование запушен");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if(bookingTaskQueue.TryDequeue(out Booking? booking))
                {
                    logger.LogInformation("Проверяем бронирование с id: {bookingId} со статусом: {Status}", booking.Id, booking.Status);
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    booking.Status = BookingStatus.Confirmed;
                    booking.ProcessedAt = DateTime.UtcNow;
                    logger.LogInformation("Бронирование c id: {bookingId} прошло успешно. Статус: {Status}", booking.Id, booking.Status);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Неизвестная ошибка бронирования");
            }
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
        logger.LogInformation("Сервис обработки задач на бронирование остановлен");
    }
}