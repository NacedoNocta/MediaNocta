using Microsoft.AspNetCore.Components;

namespace Website.Components.Shared;

public partial class Pagination
{
    [Parameter, EditorRequired] public int CurrentPage { get; set; }
    [Parameter, EditorRequired] public int TotalPages { get; set; }
    [Parameter, EditorRequired] public EventCallback<int> OnPageChange { get; set; }
    [Parameter] public string AriaLabel { get; set; } = "Pagination";

    private Task ChangePage(int page)
    {
        if (page < 1 || page > TotalPages || page == CurrentPage)
            return Task.CompletedTask;
        return OnPageChange.InvokeAsync(page);
    }
}
