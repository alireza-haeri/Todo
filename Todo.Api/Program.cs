using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.DataProtection;
using Todo.Api.Models;
using Microsoft.Data.SqlClient;
using System.Threading;

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

// EF Core: TodoDbContext (SQL Server)
builder.Services.AddDbContext<Todo.Api.Data.TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")
        ?? "Server=sqlserver;Database=TodoApiDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"));

// Data Protection keys (shared for replicas)
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
    .SetApplicationName("Todo.Api")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

// Configure JWT authentication for Todo.Api (tokens issued by Todo.Profile)
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Todo.Profile";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Todo.Web";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// NOTE: Database migrations are not applied automatically here to avoid startup-time migration conflicts in container environments.
// Run migrations separately (see the 'migrator' service in docker-compose.yml or run 'dotnet ef database update' locally).

app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

// Minimal health endpoint
app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy", machineName = Environment.MachineName }));

// CRUD APIs for Todos (require authentication)
app.MapGet("/api/todos", async (Todo.Api.Data.TodoDbContext db, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    if (userId is null) return Results.Unauthorized();

    var todos = await db.Todos.Where(t => t.UserId == userId).ToListAsync();
    return Results.Ok(todos);
}).RequireAuthorization();

app.MapGet("/api/todos/{id}", async (int id, Todo.Api.Data.TodoDbContext db, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    if (userId is null) return Results.Unauthorized();

    var todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
}).RequireAuthorization();

app.MapPost("/api/todos", async (TodoItem input, Todo.Api.Data.TodoDbContext db, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    if (userId is null) return Results.Unauthorized();

    var todo = new TodoItem
    {
        Title = input.Title,
        IsCompleted = input.IsCompleted,
        UserId = userId
    };

    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
}).RequireAuthorization();

app.MapPut("/api/todos/{id}", async (int id, TodoItem input, Todo.Api.Data.TodoDbContext db, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    if (userId is null) return Results.Unauthorized();

    var todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    if (todo is null) return Results.NotFound();

    todo.Title = input.Title;
    todo.IsCompleted = input.IsCompleted;
    await db.SaveChangesAsync();
    return Results.Ok(todo);
}).RequireAuthorization();

app.MapDelete("/api/todos/{id}", async (int id, Todo.Api.Data.TodoDbContext db, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    if (userId is null) return Results.Unauthorized();

    var todo = await db.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();