var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL Server Instance
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin();

// PGSQL Databases
var mainDatabase = postgres.AddDatabase("mainDatabase");
var keycloakDatabase = postgres.AddDatabase("keycloakDatabase");
var websiteDatabase = postgres.AddDatabase("websiteDatabase");
var backofficeDatabase = postgres.AddDatabase("backofficeDatabase");

// Keycloak Identity Provider
// Realm is imported from ./keycloak-realm on fresh starts (when the data volume is empty).
// The realm export contains development client secrets — do not use this setup in production.
var keycloak = builder.AddKeycloak("keycloak", port: 8080)
    .WithDataVolume()
    .WithRealmImport("./keycloak-realm")
    .WaitFor(keycloakDatabase);

// Database Manager
// Handles migrations for mainDatabase (content), websiteDatabase (auth), and backofficeDatabase (auth)
var databaseManager = builder.AddProject<Projects.DatabaseManager>("databasemanager")
    .WithReference(mainDatabase)
    .WithReference(websiteDatabase)
    .WithReference(backofficeDatabase)
    .WaitFor(mainDatabase)
    .WaitFor(websiteDatabase)
    .WaitFor(backofficeDatabase)
    .WithExplicitStart();

var databaseSeeder = builder.AddProject<Projects.DatabaseSeeder>("databaseseeder")
    .WithReference(mainDatabase)
    .WaitFor(mainDatabase)
    .WithExplicitStart();

// Blog Api
var blogApiProject = builder.AddProject<Projects.BlogApi>("blogapi")
    .WithReference(mainDatabase)
    .WaitFor(mainDatabase);

// Activity Api
var activityApiProject = builder.AddProject<Projects.ActivityAPI>("activityapi")
    .WithReference(mainDatabase)
    .WaitFor(mainDatabase);

// Fragment Api
var fragmentApiProject = builder.AddProject<Projects.FragmentAPI>("fragmentapi")
    .WithReference(mainDatabase)
    .WaitFor(mainDatabase);

// Tech Api
var techApiProject = builder.AddProject<Projects.TechAPI>("techapi")
    .WithReference(mainDatabase)
    .WaitFor(mainDatabase);

// API Gateway
var apiGatewayProject = builder.AddProject<Projects.APIGateway>("gateway")
    .WithReference(activityApiProject)
    .WithReference(blogApiProject)
    .WithReference(fragmentApiProject)
    .WithReference(techApiProject)
    .WithReference(keycloak)
    .WaitFor(activityApiProject)
    .WaitFor(blogApiProject)
    .WaitFor(fragmentApiProject)
    .WaitFor(techApiProject)
    .WaitFor(keycloak)
    .WithExternalHttpEndpoints();

// Main Website
// Uses websiteDatabase for auth entities (separate from APIs' mainDatabase)
var website = builder.AddProject<Projects.Website>("website")
    .WithReference(apiGatewayProject)
    .WithReference(websiteDatabase)
    .WithReference(keycloak)
    .WaitFor(apiGatewayProject)
    .WaitFor(websiteDatabase)
    .WithExternalHttpEndpoints();

// Backoffice
// Uses backofficeDatabase for auth entities (separate from Website)
var backoffice = builder.AddProject<Projects.Backoffice>("backoffice")
    .WithReference(apiGatewayProject)
    .WithReference(backofficeDatabase)
    .WithReference(keycloak)
    .WaitFor(apiGatewayProject)
    .WaitFor(backofficeDatabase)
    .WithExternalHttpEndpoints();

builder.Build().Run();
