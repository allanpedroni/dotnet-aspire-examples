using AspireApp.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithClearCacheCommand();

if (builder.ExecutionContext.IsRunMode)
{
    var sql = builder.AddSqlServer("sql")
        .WithLifetime(ContainerLifetime.Persistent);

    var sqldb = sql.AddDatabase("sqldb");

    var apiService = builder
        .AddProject<Projects.AspireApp_ApiService>(
            "apiservice")
        .WaitFor(sqldb)
        .WithReference(sqldb);

    builder.AddProject<Projects.AspireApp_Web>("webfrontend")
        .WithExternalHttpEndpoints()
        .WithReference(cache)
        .WithReference(apiService)
        .WaitFor(apiService)
        .WithReplicas(3);
}
else
{
    var sql = builder.AddAzureSqlServer("sql");
    var sqldb = sql.AddDatabase("sqldb");

    var apiService = builder
        .AddProject<Projects.AspireApp_ApiService>(
            "apiservice")
        .WaitFor(sqldb)
        .WithReference(sqldb);

    builder.AddProject<Projects.AspireApp_Web>("webfrontend")
        .WithExternalHttpEndpoints()
        .WithReference(cache)
        .WithReference(apiService)
        .WaitFor(apiService)
        .WithReplicas(3);
}

builder.Build().Run();
