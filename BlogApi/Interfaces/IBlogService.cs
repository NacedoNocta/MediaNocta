using BlogLibrary.Interfaces;

namespace BlogApi.Interfaces;

public interface IBlogService
{
    Task<IEnumerable<IBlog>> GetBlogPostsAsync(uint page = 1, uint pageSize = 10);
    Task<IBlog?> GetBlogPostByIdAsync(Guid id);
    Task<uint> GetBlogPostCountAsync();
    Task<IBlog> CreateBlogPostAsync(IBlog blog);
    Task<IBlog?> UpdateBlogPostAsync(IBlog blog);
    Task<bool> DeleteBlogPostAsync(Guid id);
    Task<IEnumerable<IBlog>> GetBlogPostsByAuthorAsync(Guid authorId);
    Task<IEnumerable<IBlog>> GetBlogPostsByTagAsync(Guid tagId);
}