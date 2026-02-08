namespace Todo.Profile.Models;

public record RegisterRequest(string Email, string Password);
public record RegisterResponse(string Message);

public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, DateTime ExpiresAt);
