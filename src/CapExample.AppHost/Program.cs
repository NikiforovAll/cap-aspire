var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.ExecutionContext.IsPublishMode
    ? builder.AddSqlServer("sqlserver").PublishAsAzureSqlDatabase().AddDatabase("sqldb")
    : builder.AddConnectionString("sqldb");

var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("serviceBus")
    : builder.AddConnectionString("serviceBus");

builder.AddProject<Projects.Consumer>("consumer")
    .WithReference(serviceBus);

builder.AddProject<Projects.Producer>("producer")
    .WithReference(sqlServer)
    .WithReference(serviceBus);

builder.Build().Run();
