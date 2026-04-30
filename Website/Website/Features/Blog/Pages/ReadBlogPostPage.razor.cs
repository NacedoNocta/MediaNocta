using System.Web;
using BlogLibrary;
using BlogLibrary.Interfaces;
using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Website.Features.Blog.Pages;

public partial class ReadBlogPostPage : IAsyncDisposable
{
    [Parameter]
    public string blogid { get; set; } = string.Empty;

    private BlogLibrary.Blog? post = null;
    private bool isLoading = true;
    private bool hasError = false;
    private IJSObjectReference? _module;

    protected override async Task OnInitializedAsync()
    {
        await LoadBlogPostAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _module = await JSRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./Features/Blog/Pages/ReadBlogPostPage.razor.js");
        }

        if (_module is not null)
        {
            await _module.InvokeVoidAsync("scrollToElement", "contentTop");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); }
            catch (JSDisconnectedException) { }
        }
    }

    private async Task LoadBlogPostAsync()
    {
        try
        {
            isLoading = true;
            hasError = false;
            StateHasChanged();

            if (Guid.TryParse(blogid, out var blogGuid))
            {
                post = await BlogService.GetPostByIdAsync(blogGuid);
                Logger.LogInformation("Loaded blog post with ID {BlogId}", blogGuid);
            }
            else
            {
                Logger.LogWarning("Invalid blog ID format: {BlogId}", blogid);
                hasError = true;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading blog post with ID {BlogId}", blogid);
            hasError = true;
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private string GetLocalizedTitle()
    {
        if (post?.Title == null) return "";
        var currentLanguage = CultureService.GetCurrentCulture();
        return post.Title.GetText(currentLanguage);
    }

    private string GetLocalizedContent()
    {
        if (post?.Content == null) return "";
        var currentLanguage = CultureService.GetCurrentCulture();
        return post.Content.GetText(currentLanguage);
    }

    private string GetProcessedContent()
    {
        if (post?.Content == null) return "";

        var rawContent = GetLocalizedContent();
        if (string.IsNullOrEmpty(rawContent)) return "";

        return post.ContentType switch
        {
            ContentType.Markdown => ProcessMarkdownContent(rawContent),
            ContentType.Html => rawContent,
            ContentType.PlainText => ProcessPlainTextContent(rawContent),
            _ => rawContent
        };
    }

    private string ProcessMarkdownContent(string markdownContent)
    {
        try
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSoftlineBreakAsHardlineBreak()
                .Build();

            return Markdown.ToHtml(markdownContent, pipeline);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing Markdown content");
            return ProcessPlainTextContent(markdownContent);
        }
    }

    private static string ProcessPlainTextContent(string plainTextContent)
    {
        var encodedContent = HttpUtility.HtmlEncode(plainTextContent);
        return encodedContent.Replace("\n", "<br>").Replace("\r", "");
    }

    private string GetLocalizedBiography()
    {
        if (post?.Author.Biography == null) return "";
        var currentLanguage = CultureService.GetCurrentCulture();
        return post.Author.Biography.GetText(currentLanguage);
    }

    private static string GetTagColor(BlogLibrary.Tag tag) =>
        !string.IsNullOrEmpty(tag.Color) ? tag.Color : "secondary";

    private bool HasAuthorSocialLinks() =>
        !string.IsNullOrEmpty(post?.Author.Website) ||
        !string.IsNullOrEmpty(post?.Author.Twitter) ||
        !string.IsNullOrEmpty(post?.Author.LinkedIn) ||
        !string.IsNullOrEmpty(post?.Author.GitHub);
}
