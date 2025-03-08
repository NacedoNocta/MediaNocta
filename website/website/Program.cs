using website.Components;
using website.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddHttpClient<BlogService>(c =>
{
    var url = builder.Configuration["BlogEndpoint"] ?? throw new InvalidOperationException("BlogEndpoint is not set");
    c.BaseAddress = new(url);
});
builder.Services.AddHttpClient<ActivityService>(c =>
{
    var url = builder.Configuration["ActivityEndpoint"] ?? throw new InvalidOperationException("ActivityEndpoint is not set");
    c.BaseAddress = new(url);
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(website.Client._Imports).Assembly);

app.Run();
