using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Todo.Web.Models;
using Todo.Web.Services;

namespace Todo.Web.pages.Todos;

public class IndexModel(ITodoApiClient userClient) : PageModel
{
    public List<TodoItem> Items { get; private set; } = new();

    [BindProperty]
    public string NewTitle { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var res = await userClient.GetAsync<List<TodoItem>>("/api/todos");
        Items = res ?? [];
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTitle)) return RedirectToPage();

        var toCreate = new TodoItem { Title = NewTitle };
        var created = await userClient.PostAsync<TodoItem, TodoItem>("/api/todos", toCreate);
        return RedirectToPage();
    }
}