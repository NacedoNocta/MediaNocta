using FragmentLibrary;
using Microsoft.AspNetCore.Components;
using Website.Features.Fragment.Components;

namespace Website.Features.Fragment.Pages;

public partial class FragmentsPage
{
    private List<FragmentLibrary.Fragment> fragments = new();
    private int currentPage = 1;
    private int pageSize = 6;
    private int totalPages = 1;
    private bool isLoading = true;
    private bool hasError = false;
    private Guid? expandedFragmentId = null;
    private bool isSearchMode = false;
    private bool isFilterMode = false;
    private int totalFragmentsCount = 0;
    private FragmentTypeFilter? typeFilterRef;

    protected override async Task OnInitializedAsync()
    {
        await LoadFragmentsAsync();
    }

    private async Task LoadFragmentsAsync()
    {
        try
        {
            isLoading = true;
            hasError = false;
            StateHasChanged();

            fragments = await FragmentService.GetFragmentsAsync(currentPage, pageSize);

            totalPages = fragments.Count < pageSize ? currentPage : currentPage + 1;
            totalFragmentsCount = fragments.Count;

            Logger.LogInformation("Loaded {FragmentCount} fragments for page {Page}", fragments.Count, currentPage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading fragments");
            hasError = true;
            fragments = new List<FragmentLibrary.Fragment>();
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private async Task GoToPageAsync(int page)
    {
        if (page < 1 || page == currentPage)
            return;

        expandedFragmentId = null;
        currentPage = page;
        await LoadFragmentsAsync();
    }

    private async Task HandleVibeClicked(FragmentLibrary.Fragment fragment)
    {
        try
        {
            var success = await FragmentService.AddVibeAsync(fragment.Id);
            if (success)
            {
                fragment.VibeCount++;
                StateHasChanged();
                Logger.LogInformation("Vibed with fragment {Title}!", fragment.Title);
            }
            else
            {
                Logger.LogWarning("Failed to add vibe to fragment {FragmentId}", fragment.Id);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding vibe to fragment {FragmentId}", fragment.Id);
        }
    }

    private Task HandleExpandClicked(FragmentLibrary.Fragment fragment)
    {
        expandedFragmentId = fragment.Id;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleCollapseClicked(FragmentLibrary.Fragment fragment)
    {
        expandedFragmentId = null;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task HandleSearchResults(List<FragmentLibrary.Fragment> searchResults)
    {
        isSearchMode = true;
        isFilterMode = false;
        fragments = searchResults;
        expandedFragmentId = null;
        currentPage = 1;
        totalPages = 1;
        typeFilterRef?.ResetFilter();
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task HandleClearSearch()
    {
        isSearchMode = false;
        isFilterMode = false;
        expandedFragmentId = null;
        currentPage = 1;
        await LoadFragmentsAsync();
    }

    private Task HandleFilterResults(List<FragmentLibrary.Fragment> filterResults)
    {
        isFilterMode = true;
        isSearchMode = false;
        fragments = filterResults;
        expandedFragmentId = null;
        currentPage = 1;
        totalPages = 1;
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task HandleShowAll()
    {
        isFilterMode = false;
        isSearchMode = false;
        expandedFragmentId = null;
        currentPage = 1;
        await LoadFragmentsAsync();
    }
}
