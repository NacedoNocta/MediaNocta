using BlogLibrary;
using Microsoft.AspNetCore.Components;

namespace Website.Features.Blog.Pages;

public partial class BlogListPage
{
    private List<BlogLibrary.Blog> _blogs = new();
    private uint _currentPage = 1;
    private readonly uint _pageSize = 6;
    private uint _totalPosts = 0;
    private uint _totalPages = 0;
    private bool _isLoading = true;
    private bool _hasError = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadPostsAsync();
    }

    private async Task LoadPostsAsync()
    {
        try
        {
            _isLoading = true;
            _hasError = false;
            StateHasChanged();

            _totalPosts = await BlogService.GetPostCountAsync();
            _totalPages = _totalPosts > 0 ? (uint)Math.Ceiling((double)_totalPosts / _pageSize) : 0;

            _blogs = await BlogService.GetPostListAsync(_currentPage, _pageSize);

            Logger.LogInformation("Loaded {PostCount} posts for page {Page}", _blogs.Count, _currentPage);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading blog posts");
            _hasError = true;
            _blogs = new List<BlogLibrary.Blog>();
        }
        finally
        {
            _isLoading = false;
            StateHasChanged();
        }
    }

    private async Task GoToPageAsync(uint page)
    {
        if (page < 1 || page > _totalPages || page == _currentPage)
            return;

        _currentPage = page;
        await LoadPostsAsync();
        Navigation.NavigateTo($"{Navigation.Uri.Split('#')[0]}#contentTop", forceLoad: false);
    }
}
