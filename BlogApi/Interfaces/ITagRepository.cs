using BlogLibrary.Interfaces;

namespace BlogApi.Interfaces;

public interface ITagRepository
{
    Task<IEnumerable<ITag>> GetAllAsync();
    Task<ITag?> GetByIdAsync(Guid id);
    Task<ITag> CreateAsync(ITag tag);
    Task<ITag?> UpdateAsync(ITag tag);
    Task<bool> DeleteAsync(Guid id);
    Task<ITag?> GetByNameAsync(string name);
    Task<IEnumerable<ITag>> GetTagsByBlogIdAsync(Guid blogId);
}