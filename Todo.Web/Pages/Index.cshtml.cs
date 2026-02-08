using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Todo.Web.Services;

namespace Todo.Web.pages;

public class Index(ITodoApiClient todoClient,IUserApiClient userClient) : PageModel
{
    public string TodoHealth { get; set; }
    public string UserHealth { get; set; }

    public async Task OnGetAsync()
    {
        TodoHealth = await todoClient.GetStringAsync("/api/health");
        UserHealth = await userClient.GetStringAsync("/api/health");
    }
}