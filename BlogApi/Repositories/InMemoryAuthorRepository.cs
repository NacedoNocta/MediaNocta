using BlogLibrairy.Interfaces;

namespace BlogAPI.Repositories;

public class InMemoryAuthorRepository : IAuthorRepository
{
    private readonly List<IAuthor> _authors = new();

    public Task<IEnumerable<IAuthor>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<IAuthor>>(_authors);
    }

    public Task<IAuthor?> GetByIdAsync(Guid id)
    {
        var author = _authors.FirstOrDefault(a => a.Id == id);
        return Task.FromResult(author);
    }

    public Task<IAuthor> CreateAsync(IAuthor author)
    {
        _authors.Add(author);
        return Task.FromResult(author);
    }

    public Task<IAuthor?> UpdateAsync(IAuthor author)
    {
        var existingAuthor = _authors.FirstOrDefault(a => a.Id == author.Id);
        if (existingAuthor == null)
            return Task.FromResult<IAuthor?>(null);

        var index = _authors.IndexOf(existingAuthor);
        _authors[index] = author;
        return Task.FromResult<IAuthor?>(author);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var author = _authors.FirstOrDefault(a => a.Id == id);
        if (author == null)
            return Task.FromResult(false);

        _authors.Remove(author);
        return Task.FromResult(true);
    }

    public Task<IAuthor?> GetByNameAsync(string name)
    {
        var author = _authors.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(author);
    }
}