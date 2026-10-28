using EventBookingService.Services;
using EventBookingService.Repositories;
using EventBookingService.Middleware;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Values
                .SelectMany(value => value.Errors)
                .Select(error =>
                    string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "Некорректное значение."
                        : error.ErrorMessage);

            var detail = string.Join(" ", errors);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Detail = detail,
                Instance = context.HttpContext.Request.Path
            };

            return new BadRequestObjectResult(problemDetails);
        };
    });

builder.Services.AddSingleton<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

