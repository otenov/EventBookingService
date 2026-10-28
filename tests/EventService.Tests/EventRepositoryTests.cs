using EventBookingService.DTOs;
using EventBookingService.Models;
using EventBookingService.Repositories;

namespace EventBookingService.Tests;

public class EventRepositoryTests
{
    private readonly EventRepository _repository;

    private static readonly DateTime BaseDate =
        new DateTime(2026, 10, 1, 10, 0, 0);

    public EventRepositoryTests()
    {
        _repository = new EventRepository();
    }

    private static Event BuildEvent(
        string title = "test",
        int dayOffset = 0)
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

    private void SaveEvents(params Event[] events)
    {
        foreach (var @event in events)
        {
            _repository.Save(@event);
        }
    }

    [Fact]
    public void GetEvents_NoFilters_ReturnsAllEvents()
    {
        // Arrange
        var event1 = BuildEvent("event 1", 0);
        var event2 = BuildEvent("event 2", 1);

        SaveEvents(event1, event2);

        var eventQuery = new EventQuery();

        // Act
        var result = _repository.GetEvents(eventQuery);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Events.Count);

        Assert.Contains(
            result.Events,
            e => e.Id == event1.Id);

        Assert.Contains(
            result.Events,
            e => e.Id == event2.Id);
    }

    [Theory]
    [InlineData("event", 2)]
    [InlineData("event 1", 1)]
    [InlineData("EVENT", 2)]
    [InlineData("event 33", 0)]
    public void GetEvents_TitleFilter_ReturnsMatchingEvents(
        string title,
        int expectedCount)
    {
        // Arrange
        var event1 = BuildEvent("event 1", 0);
        var event2 = BuildEvent("event 2", 1);

        SaveEvents(event1, event2);

        var eventQuery = new EventQuery
        {
            Title = title
        };

        // Act
        var result = _repository.GetEvents(eventQuery);

        // Assert
        Assert.Equal(expectedCount, result.TotalCount);
        Assert.Equal(expectedCount, result.Events.Count);

        Assert.All(
            result.Events,
            e => Assert.True(
                e.Title.Contains(
                    title,
                    StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void GetEvents_FromFilter_ReturnsEventsStartingNotEarlierThanFrom()
    {
        // Arrange
        SaveEvents(
            BuildEvent("event 1", 0),
            BuildEvent("event 2", 1),
            BuildEvent("event 3", 2));

        var from = BaseDate.AddDays(1);

        var eventQuery = new EventQuery
        {
            From = from
        };

        // Act
        var result = _repository.GetEvents(eventQuery);

        // Assert
        Assert.Equal(2, result.TotalCount);

        Assert.All(
            result.Events,
            e => Assert.True(e.StartAt >= from));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 3)]
    public void GetEvents_ToFilter_ReturnsExpectedCount(
        int toDaysOffset,
        int expectedCount)
    {
        // Arrange
        SaveEvents(
            BuildEvent("event 1", 0),
            BuildEvent("event 2", 1),
            BuildEvent("event 3", 2));

        var to = BaseDate.AddDays(toDaysOffset);

        var eventQuery = new EventQuery
        {
            To = to
        };

        // Act
        var result = _repository.GetEvents(eventQuery);

        // Assert
        Assert.Equal(expectedCount, result.TotalCount);
        Assert.Equal(expectedCount, result.Events.Count);

        Assert.All(
            result.Events,
            e => Assert.True(e.EndAt <= to));
    }

    [Fact]
    public void GetEvents_Pagination_ReturnsRequestedPage()
    {
        // Arrange
        var event1 = BuildEvent("event 1", 0);
        var event2 = BuildEvent("event 2", 1);
        var event3 = BuildEvent("event 3", 2);
        var event4 = BuildEvent("event 4", 3);

        SaveEvents(
            event1,
            event2,
            event3,
            event4);

        var eventQuery = new EventQuery
        {
            Page = 2,
            PageSize = 2
        };

        // Act
        var result = _repository.GetEvents(eventQuery);

        // Assert
        Assert.Equal(4, result.TotalCount);
        Assert.Equal(2, result.Events.Count);

        Assert.Contains(
            result.Events,
            e => e.Id == event3.Id);

        Assert.Contains(
            result.Events,
            e => e.Id == event4.Id);
    }

    [Theory]
    [InlineData(1, 2, 2)]
    [InlineData(2, 2, 2)]
    [InlineData(3, 2, 0)]
    [InlineData(1, 3, 3)]
    [InlineData(2, 3, 1)]
    public void GetEvents_Pagination_ReturnsExpectedCount(
        int page,
        int pageSize,
        int expectedCount)
    {
        // Arrange
        for (var i = 1; i <= 4; i++)
        {
            _repository.Save(
                BuildEvent(
                    $"event {i}",
                    i - 1));
        }

        var eventQuery = new EventQuery
        {
            Page = page,
            PageSize = pageSize
        };

        // Act
        var result = _repository.GetEvents(eventQuery);

        // Assert
        Assert.Equal(4, result.TotalCount);
        Assert.Equal(expectedCount, result.Events.Count);
    }

    [Fact]
    public void GetEvents_CombinedFilters_ReturnsMatchingEvents()
    {
        // Arrange

        // Не проходит From
        var event1 = BuildEvent(
            "event 1",
            0);

        // Проходит все фильтры
        var event2 = BuildEvent(
            "event 2",
            1);

        // Даты подходят, но Title не подходит
        var event3 = BuildEvent(
            "meeting",
            1);

        // Title подходит, но дата слишком поздняя
        var event4 = BuildEvent(
            "event 4",
            3);

        SaveEvents(
            event1,
            event2,
            event3,
            event4);

        var eventQuery = new EventQuery
        {
            Title = "event",
            From = BaseDate.AddDays(1),
            To = BaseDate.AddDays(2)
        };

        // Act
        var result = _repository.GetEvents(eventQuery);

        // Assert
        Assert.Single(result.Events);
        Assert.Equal(event2.Id, result.Events[0].Id);

        Assert.All(
            result.Events,
            e =>
            {
                Assert.True(
                    e.Title.Contains(
                        "event",
                        StringComparison.OrdinalIgnoreCase));

                Assert.True(
                    e.StartAt >= eventQuery.From);

                Assert.True(
                    e.EndAt <= eventQuery.To);
            });
    }
}