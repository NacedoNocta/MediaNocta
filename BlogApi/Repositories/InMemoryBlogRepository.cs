using BlogLibrairy;
using BlogLibrairy.Interfaces;

namespace BlogAPI.Repositories;

public class InMemoryBlogRepository : IBlogRepository
{
    private readonly List<IBlog> _blogs = new();

    public InMemoryBlogRepository()
    {
        InitializeSampleData();
    }

    private void InitializeSampleData()
    {
        var author = new Author("John Doe", "Experienced software developer and technical writer", "https://placehold.co/150x150");
        
        var sampleBlogs = Enumerable.Range(1, 122).Select(index => new Blog(
            $"Understanding Modern Web Development - Part {index}",
            $"This is a comprehensive guide covering the fundamentals of modern web development, focusing on best practices and current trends. Part {index} of our series.",
            $"Detailed content about modern web development practices, frameworks, and methodologies. This comprehensive guide will help you understand the current landscape of web development and provide practical insights for building robust applications. Content for part {index}...",
            author,
            null,
            imageUrl: $"https://placehold.co/600x400?text=Blog+Post+{index}")
        ).ToList();

        _blogs.AddRange(sampleBlogs);
    }

    public Task<IEnumerable<IBlog>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var blogs = _blogs.Skip(skip).Take(pageSize);
        return Task.FromResult(blogs);
    }

    public Task<IBlog?> GetByIdAsync(Guid id)
    {
        var blog = _blogs.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(blog);
    }

    public Task<uint> GetCountAsync()
    {
        return Task.FromResult((uint)_blogs.Count);
    }

    public Task<IBlog> CreateAsync(IBlog blog)
    {
        _blogs.Add(blog);
        return Task.FromResult(blog);
    }

    public Task<IBlog?> UpdateAsync(IBlog blog)
    {
        var existingBlog = _blogs.FirstOrDefault(b => b.Id == blog.Id);
        if (existingBlog == null)
            return Task.FromResult<IBlog?>(null);

        var index = _blogs.IndexOf(existingBlog);
        _blogs[index] = blog;
        return Task.FromResult<IBlog?>(blog);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var blog = _blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null)
            return Task.FromResult(false);

        _blogs.Remove(blog);
        return Task.FromResult(true);
    }

    public Task<IEnumerable<IBlog>> GetByAuthorIdAsync(Guid authorId)
    {
        var blogs = _blogs.Where(b => b.Author.Id == authorId);
        return Task.FromResult(blogs);
    }

    public Task<IEnumerable<IBlog>> GetByTagIdAsync(Guid tagId)
    {
        var blogs = _blogs.Where(b => b.Tags.Any(t => t.Id == tagId));
        return Task.FromResult(blogs);
    }
}