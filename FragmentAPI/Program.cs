var builder = WebApplication.CreateBuilder(args);

// Add service defaults from the shared library
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map default endpoints from service defaults
app.MapDefaultEndpoints();

// Map controllers
app.MapControllers();

app.Run();