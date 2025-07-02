var builder = DistributedApplication.CreateBuilder(args);

// Database Manager
var databaseManager = builder.AddProject<Projects.DatabaseManager>("databasemanager");

// Blog Api
var blogApiProject = builder.AddProject<Projects.BlogApi>("blogapi");

// Activity Api
var activityApiProject = builder.AddProject<Projects.ActivityAPI>("activityapi");

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
