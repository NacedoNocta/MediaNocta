var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL Database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var postgresDb = postgres.AddDatabase("medianocta");

// Keycloak Identity Provider
var keycloak = builder.AddKeycloak("keycloak", port: 8080)
    .WithDataVolume();

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
    .WithReference(keycloak)
    .WaitFor(activityApiProject)
    .WaitFor(blogApiProject)
    .WaitFor(keycloak)
    .WithExternalHttpEndpoints();

// Main Website
var website = builder.AddProject<Projects.Website>("website")
    .WithReference(apiGatewayProject)
    .WithReference(keycloak)
    .WaitFor(apiGatewayProject)
    .WithExternalHttpEndpoints();


builder.Build().Run();
