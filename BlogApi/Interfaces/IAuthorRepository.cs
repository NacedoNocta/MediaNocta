using BlogLibrary.Interfaces;

namespace BlogApi.Interfaces;

public interface IAuthorRepository
{
    Task<IEnumerable<IAuthor>> GetAllAsync();
    Task<IAuthor?> GetByIdAsync(Guid id);
    Task<IAuthor> CreateAsync(IAuthor author);
    Task<IAuthor?> UpdateAsync(IAuthor author);
    Task<bool> DeleteAsync(Guid id);
    Task<IAuthor?> GetByNameAsync(string name);
}