using DotNetCore.CAP;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBus("serviceBus");
builder.AddNpgsqlDataSource("postgresdb_producer");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

builder.Services.AddCap(x =>
{
    var postgresConnection = builder.Configuration.GetConnectionString("postgresdb_producer")!;
    var serviceBusConnection = builder.Configuration.GetConnectionString("serviceBus")!;

    x.UseAzureServiceBus(serviceBusConnection);
    x.UsePostgreSql(x => x.DataSource = NpgsqlDataSource.Create(postgresConnection));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapPost("/send", async (NpgsqlDataSource dataSource, ICapPublisher capPublisher, TimeProvider timeProvider) =>
{
    using var connection = dataSource.OpenConnection();
    using var transaction = connection.BeginTransaction(capPublisher, autoCommit: false);

    var command = connection.CreateCommand();
    command.Transaction = (NpgsqlTransaction)transaction.DbTransaction!;
    command.CommandText = "SELECT NOW()";
    var serverTime = await command.ExecuteScalarAsync();

    await capPublisher.PublishDelayAsync(
        TimeSpan.FromMilliseconds(500),
        "test.show.time",
        new MyMessage(serverTime.ToString()));

    transaction.Commit();
});

app.MapDefaultEndpoints();

app.Run();

public record MyMessage(string CreatedAt);
