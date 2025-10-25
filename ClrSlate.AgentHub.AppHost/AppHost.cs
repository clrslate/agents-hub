var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo")
                 .WithMongoExpress()
                 .WithDataVolume();

var todoDb = mongo.AddDatabase("todo");

var apiService = builder.AddProject<Projects.ClrSlate_AgentHub_ApiService>("apiservice")
        .WithReference(todoDb).WaitFor(todoDb)
        .WithHttpHealthCheck("/health");

builder.AddProject<Projects.ClrSlate_AgentHub_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
