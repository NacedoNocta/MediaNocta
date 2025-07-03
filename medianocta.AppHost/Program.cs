var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL Database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var postgresDb = postgres.AddDatabase("medianocta");

// Database Manager
var databaseManager = builder.AddProject<Projects.DatabaseManager>("databasemanager")
    .WithReference(postgresDb);

// Blog Api
var blogApiProject = builder.AddProject<Projects.BlogApi>("blogapi")
    .WithReference(postgresDb);

// Activity Api
var activityApiProject = builder.AddProject<Projects.ActivityAPI>("activityapi")
    .WithReference(postgresDb);

// API Gateway
var apiGatewayProject = builder.AddProject<Projects.APIGateway>("gateway")
    .WithReference(activityApiProject)
    .WithReference(blogApiProject)
    .WaitFor(activityApiProject)
    .WaitFor(blogApiProject)
    .WithExternalHttpEndpoints();

// Main Website
var website = builder.AddProject<Projects.website>("website")
    .WithReference(apiGatewayProject)
    .WaitFor(apiGatewayProject)
    .WithExternalHttpEndpoints();


builder.Build().Run();
