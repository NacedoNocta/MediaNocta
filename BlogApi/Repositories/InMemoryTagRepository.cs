using BlogLibrairy.Interfaces;

namespace BlogAPI.Repositories;

public class InMemoryTagRepository : ITagRepository
{
    private readonly List<ITag> _tags = new();

    public Task<IEnumerable<ITag>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<ITag>>(_tags);
    }

    public Task<ITag?> GetByIdAsync(Guid id)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(tag);
    }

    public Task<ITag> CreateAsync(ITag tag)
    {
        _tags.Add(tag);
        return Task.FromResult(tag);
    }

    public Task<ITag?> UpdateAsync(ITag tag)
    {
        var existingTag = _tags.FirstOrDefault(t => t.Id == tag.Id);
        if (existingTag == null)
            return Task.FromResult<ITag?>(null);

        var index = _tags.IndexOf(existingTag);
        _tags[index] = tag;
        return Task.FromResult<ITag?>(tag);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var tag = _tags.FirstOrDefault(t => t.Id == id);
        if (tag == null)
            return Task.FromResult(false);

        _tags.Remove(tag);
        return Task.FromResult(true);
    }

    public Task<ITag?> GetByNameAsync(string name)
    {
        var tag = _tags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(tag);
    }

    public Task<IEnumerable<ITag>> GetTagsByBlogIdAsync(Guid blogId)
    {
        return Task.FromResult<IEnumerable<ITag>>(_tags);
    }
}