using BlogLibrary.Interfaces;

namespace BlogApi.Interfaces;

public interface IAuthorService
{
    Task<IEnumerable<IAuthor>> GetAllAuthorsAsync();
    Task<IAuthor?> GetAuthorByIdAsync(Guid id);
    Task<IAuthor> CreateAuthorAsync(IAuthor author);
    Task<IAuthor?> UpdateAuthorAsync(IAuthor author);
    Task<bool> DeleteAuthorAsync(Guid id);
    Task<IAuthor?> GetAuthorByNameAsync(string name);
}
