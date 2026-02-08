using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Data.SqlClient;
using Todo.Profile.Data;
using Todo.Profile.Models;
using Todo.Profile.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "*" };
builder.Services.AddCors(options =>
{
	options.AddPolicy("DefaultCorsPolicy", policy =>
	{
		if (allowedOrigins.Length == 1 && allowedOrigins[0] == "*")
		{
			policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
		}
		else
		{
			policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
		}
	});
});

builder.Services.AddDataProtection()
	.PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
	.SetApplicationName("Todo.Profile")
	.SetDefaultKeyLifetime(TimeSpan.FromDays(90));

// EF Core + Identity
builder.Services.AddDbContext<ProfileDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("Default")
		?? "Server=sqlserver;Database=TodoProfileDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"));

// Apply migrations with retry to handle DB startup race in containerized environments
builder.Services.BuildServiceProvider();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
	{
		options.User.RequireUniqueEmail = true;
	})
	.AddEntityFrameworkStores<ProfileDbContext>()
	.AddSignInManager();


// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required");
var issuer = builder.Configuration["Jwt:Issuer"] ?? "Todo.Profile";
var audience = builder.Configuration["Jwt:Audience"] ?? "Todo.Web";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidateLifetime = true,
			ValidIssuer = issuer,
			ValidAudience = audience,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
		};
	});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

var app = builder.Build();

// migrations with retry
// NOTE: Database migrations are not applied automatically here to avoid startup-time migration conflicts in container environments.
// Run migrations separately (see the 'migrator' service in docker-compose.yml or run 'dotnet ef database update' locally).

app.UseRouting();
app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy" }));

app.MapPost("/api/auth/register", async ([FromBody]RegisterRequest request, [FromServices]UserManager<ApplicationUser> userManager,[FromServices]ILogger<Program> logger) =>
	{
		try
		{
			logger.LogInformation("Registering user {email}", request.Email);

			var user = new ApplicationUser
			{
				UserName = request.Email,
				Email = request.Email
			};

			var result = await userManager.CreateAsync(user, request.Password);
			if (!result.Succeeded)
			{
				logger.LogWarning("Registering user {email} failed", request.Email);
				return Results.BadRequest(new { errors = result.Errors.Select(e => e.Description) });
			}

			logger.LogInformation("Registering user {email} successful", request.Email);
			return Results.Ok(new RegisterResponse("Registered"));
		}
		catch (Exception e)
		{
			logger.LogError(e, "Registering user {email} failed", request.Email);
			throw e;
		}
	})
	.WithName("Register");

app.MapPost("/api/auth/login", async (LoginRequest request, UserManager<ApplicationUser> userManager, IJwtTokenService tokenService) =>
	{
		var user = await userManager.FindByEmailAsync(request.Email);
		if (user is null)
		{
			return Results.Unauthorized();
		}

		var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
		if (!validPassword)
		{
			return Results.Unauthorized();
		}

		var (token, expiresAt) = tokenService.CreateToken(user);
		return Results.Ok(new AuthResponse(token, expiresAt));
	})
	.WithName("Login");

app.MapGet("/api/profile/me", (ClaimsPrincipal user) =>
	{
		if (user.Identity?.IsAuthenticated != true)
		{
			return Results.Unauthorized();
		}

		return Results.Ok(new
		{
			userId = user.FindFirstValue(ClaimTypes.NameIdentifier),
			email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue(JwtRegisteredClaimNames.Email)
		});
	})
	.RequireAuthorization();

app.Run();
