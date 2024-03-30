using DotNetCore.CAP;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBus("serviceBus");
builder.AddSqlServerClient("sqldb");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<SampleSubscriber>();

builder.Services.AddCap(x =>
{
    var dbConnectionString = builder.Configuration.GetConnectionString("sqldb")!;
    var serviceBusConnection = builder.Configuration.GetConnectionString("serviceBus")!;

    x.UseAzureServiceBus(serviceBusConnection);
    x.UseSqlServer(x => x.ConnectionString = dbConnectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapDefaultEndpoints();

app.Run();

public class SampleSubscriber(
    TimeProvider timeProvider, ILogger<SampleSubscriber> logger) : ICapSubscribe
{
    public record MyMessage(string CreatedAt);
    
    [CapSubscribe("test.show.time")]
    public async Task HandleAsync(MyMessage message)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(300), timeProvider);
        
        logger.LogInformation("Message received: {CreatedAt}", message.CreatedAt);
    }
}