namespace EventBookingService.DTOs;

using EventBookingService.Models;

public class PaginatedResult
{
    public int TotalCount {get; set;}

    public IReadOnlyList<Event> Events {get; set;} = [];

    public int Page{get; set;}

    public int Count {get; set;}
}