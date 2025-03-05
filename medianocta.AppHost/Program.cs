var builder = DistributedApplication.CreateBuilder(args);

// Blog Api
var blogapi = builder.AddProject<Projects.BlogApi>("blogapi");

// Main Website
var website = builder.AddProject<Projects.website>("website")
    .WithExternalHttpEndpoints()
    .WithReference(blogapi);

builder.Build().Run();
