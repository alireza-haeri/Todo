using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Web.Models;
using Todo.Web.Services;

namespace Todo.Web.pages.Auth;

public class RegisterModel(IUserApiClient userClient, IConfiguration config) : PageModel
{
    [BindProperty]
    public RegisterRequest Input { get; set; } = new("ali@gmail.com", "Adfkldhyf7t4gu!");

    public string? Message { get; private set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        var res = await userClient.PostAsync<RegisterRequest, RegisterResponse>($"/api/auth/register", Input);
        if (res is null)
        {
            Message = "Registration failed";
            return Page();
        }

        Message = res.Message;
        return Page();
    }
}