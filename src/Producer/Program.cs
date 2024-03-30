using DotNetCore.CAP;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBus("serviceBus");
builder.AddSqlServerClient("sqldb");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

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

app.UseExceptionHandler();

app.MapGet("/send", async (SqlConnection connection, ICapPublisher capPublisher, TimeProvider timeProvider) =>
{
    var startTime = timeProvider.GetTimestamp();
    using var transaction = connection.BeginTransaction(capPublisher, autoCommit: false);

    var command = connection.CreateCommand();
    command.Transaction = (SqlTransaction)transaction;
    command.CommandText = "SELECT GETDATE()";
    var serverTime = await command.ExecuteScalarAsync();

    await capPublisher.PublishDelayAsync(
        TimeSpan.FromMilliseconds(500),
        "test.show.time",
        new MyMessage(serverTime?.ToString()!));

    transaction.Commit();

    return TypedResults.Ok(new { Status = "Published", Duration = timeProvider.GetElapsedTime(startTime) });
});

app.MapDefaultEndpoints();

app.Run();

public record MyMessage(string CreatedAt);
