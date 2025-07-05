var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL Database
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

var postgresDb = postgres.AddDatabase("medianocta");

// Keycloak Identity Provider
var keycloak = builder.AddKeycloak("keycloak", port: 8080)
    .WithDataVolume()
    .WaitFor(postgresDb);

// Database Manager
var databaseManager = builder.AddProject<Projects.DatabaseManager>("databasemanager")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

// Blog Api
var blogApiProject = builder.AddProject<Projects.BlogApi>("blogapi")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

// Activity Api
var activityApiProject = builder.AddProject<Projects.ActivityAPI>("activityapi")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

// Fragment Api
var fragmentApiProject = builder.AddProject<Projects.FragmentAPI>("fragmentapi")
    .WithReference(postgresDb)
    .WaitFor(postgresDb);

// API Gateway
var apiGatewayProject = builder.AddProject<Projects.APIGateway>("gateway")
    .WithReference(activityApiProject)
    .WithReference(blogApiProject)
    .WithReference(fragmentApiProject)
    .WithReference(keycloak)
    .WaitFor(activityApiProject)
    .WaitFor(blogApiProject)
    .WaitFor(fragmentApiProject)
    .WaitFor(keycloak)
    .WithExternalHttpEndpoints();

// Main Website
var website = builder.AddProject<Projects.Website>("website")
    .WithReference(apiGatewayProject)
    .WithReference(keycloak)
    .WaitFor(apiGatewayProject)
    .WithExternalHttpEndpoints();


builder.Build().Run();
