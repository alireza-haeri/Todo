using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Web.Models;
using Todo.Web.Services;

namespace Todo.Web.pages.Auth;

public class LoginModel(IUserApiClient userClient, IConfiguration config) : PageModel
{
    [BindProperty]
    public LoginRequest Input { get; set; } = new("", "");

    public string? Message { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var res = await userClient.PostAsync<LoginRequest, AuthResponse>($"/api/auth/login", Input);
        if (res is null)
        {
            Message = "Login failed";
            return Page();
        }

        // Store JWT in cookie for server-side API calls
        Response.Cookies.Append("access_token", res.Token, new CookieOptions
        {
            // Keep cookie HttpOnly for security; server-side code can still read it.
            HttpOnly = true,
            Expires = res.ExpiresAt,
            SameSite = SameSiteMode.Lax
        });

        Message = "Logged in";
        return Page();
    }
}