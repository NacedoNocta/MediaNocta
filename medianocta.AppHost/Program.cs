var builder = DistributedApplication.CreateBuilder(args);

// Blog Api
var blogapi = builder.AddProject<Projects.BlogApi>("blogapi");

// Activity Api
var activityapi = builder.AddProject<Projects.ActivityAPI>("activityapi");

// Main Website
var website = builder.AddProject<Projects.website>("website")
    .WithExternalHttpEndpoints()
    .WithReference(blogapi)
    .WithReference(activityapi);

builder.Build().Run();
