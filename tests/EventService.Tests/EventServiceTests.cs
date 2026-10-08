using EventBookingService.DTOs;
using EventBookingService.Exceptions;
using EventBookingService.Models;
using EventBookingService.Repositories;
using EventBookingService.Services;
using Moq;

namespace EventBookingService.Tests;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _repositoryMock;
    private readonly EventService _service;

    private static readonly DateTime BaseDate =
        new DateTime(2026, 10, 1, 10, 0, 0);

    public EventServiceTests()
    {
        _repositoryMock = new Mock<IEventRepository>();
        _service = new EventService(_repositoryMock.Object);
    }

    private static Event BuildEvent(string title = "test", int dayOffset = 0)
    {
        var startAt = BaseDate.AddDays(dayOffset);

        return new Event
        {
            Title = title,
            Description = "test description",
            StartAt = startAt,
            EndAt = startAt.AddHours(1)
        };
    }

 
[Fact]
public void CreateEvent_ValidData_CreatesAndSavesEvent()
{
    // Arrange
    var title = "test";
    var description = "test description";
    var startAt = BaseDate;
    var endAt = startAt.AddHours(1);

    // Act
    var result = _service.CreateEvent(
        title,
        description,
        startAt,
        endAt);

    // Assert
    Assert.Equal(title, result.Title);
    Assert.Equal(description, result.Description);
    Assert.Equal(startAt, result.StartAt);
    Assert.Equal(endAt, result.EndAt);

    _repositoryMock.Verify(
        repository => repository.Save(
            It.Is<Event>(e =>
                e.Title == title &&
                e.Description == description &&
                e.StartAt == startAt &&
                e.EndAt == endAt)),
        Times.Once);
}

    [Fact]
    public void CreateEvent_InvalidDates_ThrowsValidationException()
    {
        // Arrange
        var startAt = BaseDate;
        var endAt = startAt;

        // Act + Assert
        Assert.Throws<ValidationException>(() =>
            _service.CreateEvent(
                "test",
                "test description",
                startAt,
                endAt));

        _repositoryMock.Verify(
            repository => repository.Save(It.IsAny<Event>()),
            Times.Never);
    }

    [Fact]
    public void GetEventById_ExistingId_ReturnsEvent()
    {
        // Arrange
        var @event = BuildEvent();

        _repositoryMock
            .Setup(repository => repository.GetById(@event.Id))
            .Returns(@event);

        // Act
        var result = _service.GetEventById(@event.Id);

        // Assert
        Assert.Equal(@event.Id, result.Id);

        _repositoryMock.Verify(
            repository => repository.GetById(@event.Id),
            Times.Once);
    }

    [Fact]
    public void GetEventById_NotExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var notExistingId = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.GetById(notExistingId))
            .Returns((Event?)null);

        // Act + Assert
        Assert.Throws<NotFoundException>(() =>
            _service.GetEventById(notExistingId));

        _repositoryMock.Verify(
            repository => repository.GetById(notExistingId),
            Times.Once);
    }

    [Fact]
    public void UpdateEvent_ExistingId_UpdatesEvent()
    {
        // Arrange
        var @event = BuildEvent();

        _repositoryMock
            .Setup(repository => repository.GetById(@event.Id))
            .Returns(@event);

        var updatedTitle = "updated test";
        var updatedDescription = "updated test description";
        var updatedStartAt = BaseDate.AddDays(1);
        var updatedEndAt = updatedStartAt.AddHours(1);

        // Act
        var result = _service.UpdateEvent(
            @event.Id,
            updatedTitle,
            updatedDescription,
            updatedStartAt,
            updatedEndAt);

        // Assert
        Assert.Equal(updatedTitle, result.Title);
        Assert.Equal(updatedDescription, result.Description);
        Assert.Equal(updatedStartAt, result.StartAt);
        Assert.Equal(updatedEndAt, result.EndAt);

        _repositoryMock.Verify(
            repository => repository.Update(
                It.Is<Event>(e =>
                    e.Id == @event.Id &&
                    e.Title == updatedTitle &&
                    e.Description == updatedDescription &&
                    e.StartAt == updatedStartAt &&
                    e.EndAt == updatedEndAt)),
            Times.Once);
    }

    [Fact]
    public void UpdateEvent_NotExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var notExistingId = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.GetById(notExistingId))
            .Returns((Event?)null);

        var updatedStartAt = BaseDate.AddDays(1);
        var updatedEndAt = updatedStartAt.AddHours(1);

        // Act + Assert
        Assert.Throws<NotFoundException>(() =>
            _service.UpdateEvent(
                notExistingId,
                "updated test",
                "updated test description",
                updatedStartAt,
                updatedEndAt));

        _repositoryMock.Verify(
            repository => repository.Update(It.IsAny<Event>()),
            Times.Never);
    }

    [Fact]
    public void UpdateEvent_InvalidDates_ThrowsValidationException()
    {
        // Arrange
        var @event = BuildEvent();

        var updatedStartAt = BaseDate.AddDays(1);
        var updatedEndAt = updatedStartAt;

        // Act + Assert
        Assert.Throws<ValidationException>(() =>
            _service.UpdateEvent(
                @event.Id,
                "updated test",
                "updated test description",
                updatedStartAt,
                updatedEndAt));

        _repositoryMock.Verify(
            repository => repository.Update(It.IsAny<Event>()),
            Times.Never);

        _repositoryMock.Verify(
            repository => repository.GetById(It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public void DeleteEventById_ExistingId_CallsRepositoryDelete()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.Delete(id))
            .Returns(true);

        // Act
        _service.DeleteEventById(id);

        // Assert
        _repositoryMock.Verify(
            repository => repository.Delete(id),
            Times.Once);
    }

    [Fact]
    public void DeleteEventById_NotExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var notExistingId = Guid.NewGuid();

        _repositoryMock
            .Setup(repository => repository.Delete(notExistingId))
            .Returns(false);

        // Act + Assert
        Assert.Throws<NotFoundException>(() =>
            _service.DeleteEventById(notExistingId));

        _repositoryMock.Verify(
            repository => repository.Delete(notExistingId),
            Times.Once);
    }

    [Fact]
    public void GetEvents_ValidQuery_ReturnsPaginatedResult()
    {
        // Arrange
        var event1 = BuildEvent("event 1");
        var event2 = BuildEvent("event 2", 1);

        var eventQuery = new EventQuery
        {
            Page = 1,
            PageSize = 10
        };

        var pagedData = new PagedData
        {
            Events = new[] { event1, event2 },
            TotalCount = 2
        };

        _repositoryMock
            .Setup(repository => repository.GetEvents(eventQuery))
            .Returns(pagedData);

        // Act
        var result = _service.GetEvents(eventQuery);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(2, result.Events.Count);

        _repositoryMock.Verify(
            repository => repository.GetEvents(eventQuery),
            Times.Once);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -1)]
    public void GetEvents_InvalidPagination_ThrowsValidationException(
        int page,
        int pageSize)
    {
        // Arrange
        var eventQuery = new EventQuery
        {
            Page = page,
            PageSize = pageSize
        };

        // Act + Assert
        Assert.Throws<ValidationException>(() =>
            _service.GetEvents(eventQuery));

        _repositoryMock.Verify(
            repository => repository.GetEvents(It.IsAny<EventQuery>()),
            Times.Never);
    }

    [Fact]
    public void GetEvents_FromLaterThanTo_ThrowsValidationException()
    {
        // Arrange
        var eventQuery = new EventQuery
        {
            From = BaseDate.AddDays(2),
            To = BaseDate.AddDays(1)
        };

        // Act + Assert
        Assert.Throws<ValidationException>(() =>
            _service.GetEvents(eventQuery));

        _repositoryMock.Verify(
            repository => repository.GetEvents(It.IsAny<EventQuery>()),
            Times.Never);
    }
}