var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Register a reusable API client with base address from configuration
builder.Services.AddHttpClient<Todo.Web.Services.IApiClient, Todo.Web.Services.ApiClient>(client =>
{
    var apiAddress = builder.Configuration["Api:BaseUrl"]
                     ?? throw new ArgumentNullException($"Api:BaseUrl");
    var defaultHost = builder.Configuration["Api:DefaultHost"];
    client.BaseAddress = new Uri(apiAddress);
    client.DefaultRequestHeaders.Host = defaultHost;
});

var app = builder.Build();

app.UseRouting();
app.MapRazorPages();

app.Run();