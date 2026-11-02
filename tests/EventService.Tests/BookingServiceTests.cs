using EventBookingService.DTOs;
using EventBookingService.Exceptions;
using EventBookingService.Models;
using EventBookingService.Repositories;
using EventBookingService.Services;
using Moq;

namespace EventBookingService.Tests;

public class BookingServiceTests
{
    private readonly BookingService _bookingService;

    private readonly Mock<IEventRepository> _mockEventRepository;

    private readonly Mock<IBookingTaskQueue> _mockBookingTaskQueue;

    private readonly Mock<IBookingRepository> _mockBookingRepository;

    public BookingServiceTests()
    {
        _mockEventRepository = new Mock<IEventRepository>();
        _mockBookingTaskQueue = new Mock<IBookingTaskQueue>();
        _mockBookingRepository = new Mock<IBookingRepository>();
        _bookingService = new BookingService(
            _mockBookingRepository.Object,
            _mockEventRepository.Object,
            _mockBookingTaskQueue.Object);
    }

    [Fact]
    public async Task CreateBookingAsync_ExistingEvent_CreatesPendingBooking()
    {
        // Arrange
        var existingEvent = BuildEvent();

        _mockEventRepository
            .Setup(repository => repository.GetById(existingEvent.Id))
            .Returns(existingEvent);

        // Act
        var result = await _bookingService.CreateBookingAsync(existingEvent.Id);

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(existingEvent.Id, result.EventId);
        Assert.Equal(BookingStatus.Pending, result.Status);
        Assert.NotEqual(default(DateTime), result.CreatedAt);
        Assert.Null(result.ProcessedAt);

        _mockBookingRepository.Verify(
            repository => repository.Save(
                It.Is<Booking>(booking =>
                    booking.Id == result.Id &&
                    booking.EventId == existingEvent.Id &&
                    booking.Status == BookingStatus.Pending)),
            Times.Once);

        _mockBookingTaskQueue.Verify(
            queue => queue.Enqueue(
                It.Is<Booking>(booking => booking.Id == result.Id)),
            Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_SameEvent_CreatesBookingsWithUniqueIds()
    {
        // Arrange
        const int bookingCount = 3;
        var existingEvent = BuildEvent();

        _mockEventRepository
            .Setup(repository => repository.GetById(existingEvent.Id))
            .Returns(existingEvent);

        // Act
        var bookings = new List<BookingInfoDTO>();
        for (var i = 0; i < bookingCount; i++)
        {
            bookings.Add(await _bookingService.CreateBookingAsync(existingEvent.Id));
        }

        // Assert
        Assert.Equal(bookingCount, bookings.Select(booking => booking.Id).Distinct().Count());
        Assert.All(bookings, booking =>
        {
            Assert.NotEqual(Guid.Empty, booking.Id);
            Assert.Equal(existingEvent.Id, booking.EventId);
            Assert.Equal(BookingStatus.Pending, booking.Status);
        });

        _mockBookingRepository.Verify(
            repository => repository.Save(It.IsAny<Booking>()),
            Times.Exactly(bookingCount));

        _mockBookingTaskQueue.Verify(
            queue => queue.Enqueue(It.IsAny<Booking>()),
            Times.Exactly(bookingCount));
    }

    [Fact]
    public async Task GetBookingByIdAsync_ExistingBooking_ReturnsBookingInfo()
    {
        // Arrange
        var booking = new Booking
        {
            EventId = Guid.NewGuid(),
            CreatedAt = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc)
        };

        _mockBookingRepository
            .Setup(repository => repository.GetById(booking.Id))
            .Returns(booking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(booking.Id);

        // Assert
        Assert.Equal(booking.Id, result.Id);
        Assert.Equal(booking.EventId, result.EventId);
        Assert.Equal(booking.Status, result.Status);
        Assert.Equal(booking.CreatedAt, result.CreatedAt);
        Assert.Equal(booking.ProcessedAt, result.ProcessedAt);

        _mockBookingRepository.Verify(
            repository => repository.GetById(booking.Id),
            Times.Once);
    }

    [Theory]
    [InlineData(BookingStatus.Confirmed)]
    [InlineData(BookingStatus.Rejected)]
    public async Task GetBookingByIdAsync_StatusChanged_ReturnsCurrentStatus(BookingStatus updatedStatus)
    {
        // Arrange
        var booking = new Booking
        {
            EventId = Guid.NewGuid(),
            CreatedAt = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc)
        };
        var processedAt = booking.CreatedAt.AddSeconds(10);

        _mockBookingRepository
            .Setup(repository => repository.GetById(booking.Id))
            .Returns(booking);

        // Act
        var pendingResult = await _bookingService.GetBookingByIdAsync(booking.Id);
        booking.Status = updatedStatus;
        booking.ProcessedAt = processedAt;
        var updatedResult = await _bookingService.GetBookingByIdAsync(booking.Id);

        // Assert
        Assert.Equal(BookingStatus.Pending, pendingResult.Status);
        Assert.Null(pendingResult.ProcessedAt);
        Assert.Equal(booking.Id, updatedResult.Id);
        Assert.Equal(updatedStatus, updatedResult.Status);
        Assert.Equal(processedAt, updatedResult.ProcessedAt);
    }

    [Fact]
    public async Task CreateBookingAsync_NotExistingEvent_ThrowsNotFoundException()
    {
        // Arrange
        var eventId = Guid.NewGuid();

        _mockEventRepository
            .Setup(repository => repository.GetById(eventId))
            .Returns((Event?)null);

        // Act
        var exception = await Record.ExceptionAsync(
            () => _bookingService.CreateBookingAsync(eventId));

        // Assert
        Assert.IsType<NotFoundException>(exception);

        _mockBookingRepository.Verify(
            repository => repository.Save(It.IsAny<Booking>()),
            Times.Never);

        _mockBookingTaskQueue.Verify(
            queue => queue.Enqueue(It.IsAny<Booking>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateBookingAsync_DeletedEvent_ThrowsNotFoundException()
    {
        // Arrange
        var existingEvent = BuildEvent();
        var eventRepository = new EventRepository();
        eventRepository.Save(existingEvent);
        var wasDeleted = eventRepository.Delete(existingEvent.Id);

        var bookingService = new BookingService(
            _mockBookingRepository.Object,
            eventRepository,
            _mockBookingTaskQueue.Object);

        // Act
        var exception = await Record.ExceptionAsync(
            () => bookingService.CreateBookingAsync(existingEvent.Id));

        // Assert
        Assert.True(wasDeleted);
        Assert.IsType<NotFoundException>(exception);

        _mockBookingRepository.Verify(
            repository => repository.Save(It.IsAny<Booking>()),
            Times.Never);

        _mockBookingTaskQueue.Verify(
            queue => queue.Enqueue(It.IsAny<Booking>()),
            Times.Never);
    }

    [Fact]
    public async Task GetBookingByIdAsync_NotExistingBooking_ThrowsNotFoundException()
    {
        // Arrange
        var bookingId = Guid.NewGuid();

        _mockBookingRepository
            .Setup(repository => repository.GetById(bookingId))
            .Returns((Booking?)null);

        // Act
        var exception = await Record.ExceptionAsync(
            () => _bookingService.GetBookingByIdAsync(bookingId));

        // Assert
        Assert.IsType<NotFoundException>(exception);

        _mockBookingRepository.Verify(
            repository => repository.GetById(bookingId),
            Times.Once);
    }

    private static Event BuildEvent()
    {
        var startAt = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);

        return new Event
        {
            Title = "Test event",
            StartAt = startAt,
            EndAt = startAt.AddHours(1)
        };
    }
}
