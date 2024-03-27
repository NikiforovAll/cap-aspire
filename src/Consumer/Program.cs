using DotNetCore.CAP;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBus("serviceBus");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<SampleSubscriber>();

builder.Services.AddCap(x =>
{
    var serviceBusConnection = builder.Configuration.GetConnectionString("serviceBus")!;

    x.UseAzureServiceBus(serviceBusConnection);
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

public class SampleSubscriber : ICapSubscribe
{
    public record MyMessage(string CreatedAt);
    
    [CapSubscribe("test.show.time")]
    public void Handle(MyMessage message)
    {
        Console.WriteLine($"Message {message.CreatedAt} received");
    }
}