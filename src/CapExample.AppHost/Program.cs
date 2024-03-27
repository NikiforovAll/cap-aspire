var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder
    .AddPostgres("pg")
    .AddDatabase("postgresdb");

var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("serviceBus")
    : builder.AddConnectionString("serviceBus");

builder.AddProject<Projects.Consumer>("consumer")
    .WithReference(serviceBus);

builder.AddProject<Projects.Producer>("producer")
    .WithReference(postgresdb)
    .WithReference(serviceBus);

builder.Build().Run();
