using System.Reflection;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();



app.MapGet("/weatherforecast/{number}", static async (int number, IMediator mediator) =>
{
    return await mediator.Send(new ForecastRequest(number ==0 ? 5 :number));
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

class ForecastRequest : IRequest<IEnumerable<WeatherForecast>>
{
    public int Days { get; set; }
    public ForecastRequest(int days)
    {        
        Days = days;
    }
    public class Handler : IRequestHandler<ForecastRequest, IEnumerable<WeatherForecast>>
    {
        Task<IEnumerable<WeatherForecast>> IRequestHandler<ForecastRequest, IEnumerable<WeatherForecast>>.Handle(ForecastRequest request, CancellationToken cancellationToken)
        {
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            var forecast = Enumerable.Range(1, request.Days).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ));
            return Task.FromResult(forecast);
        }
    }
}
