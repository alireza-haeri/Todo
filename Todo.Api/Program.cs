var builder = WebApplication.CreateBuilder(args);

// Configure CORS using origins from appsettings.json (Cors:AllowedOrigins)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["*"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        if (allowedOrigins is ["*"])
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        else
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("DefaultCorsPolicy");

app.MapGet("/", () => "Hello World!");

// Minimal health endpoint
app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy" , machineName = Environment.MachineName }));

app.Run();