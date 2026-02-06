using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Todo.Web.Services;

namespace Todo.Web.pages;

public class Index(IApiClient apiClient) : PageModel
{

    public string Kazem { get; set; }

    public async Task OnGetAsync()
    {
        var res = await apiClient.GetStringAsync("/api/health");
        Kazem = res;
    }
}