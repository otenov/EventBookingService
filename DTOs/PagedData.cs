using EventBookingService.Models;

namespace EventBookingService.DTOs;

public class PagedData
{
    public IReadOnlyList<Event> Events {get; set;} = [];
    public int TotalCount{get;set;}
}