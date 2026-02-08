using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Todo.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
    .SetApplicationName("Todo.Web")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

// Register HttpContextAccessor so Api client can read auth cookie per-request
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApiClient, ApiClient>();
builder.Services.AddHttpClient<ITodoApiClient,ApiClient>( client =>
{
    var apiAddress = builder.Configuration["Api:Api:BaseUrl"]
                     ?? throw new ArgumentNullException($"Api:Api:BaseUrl");
    var defaultHost = builder.Configuration["Api:Api:DefaultHost"];
    client.BaseAddress = new Uri(apiAddress);
    client.DefaultRequestHeaders.Host = defaultHost;
});
builder.Services.AddHttpClient<IUserApiClient,ApiClient>( client =>
{
    var apiAddress = builder.Configuration["Api:Profile:BaseUrl"]
                     ?? throw new ArgumentNullException($"Api:Profile:BaseUrl");
    var defaultHost = builder.Configuration["Api:Profile:DefaultHost"];
    client.BaseAddress = new Uri(apiAddress);
    client.DefaultRequestHeaders.Host = defaultHost;
});

var app = builder.Build();

app.UseRouting();
app.MapRazorPages();

app.Run();