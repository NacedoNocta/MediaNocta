using BlogApi.Interfaces;
using BlogLibrary.Interfaces;

namespace BlogApi.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ILogger<AuthorService> _logger;

    public AuthorService(IAuthorRepository authorRepository, ILogger<AuthorService> logger)
    {
        _authorRepository = authorRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<IAuthor>> GetAllAuthorsAsync()
    {
        _logger.LogInformation("Retrieving all authors");
        return await _authorRepository.GetAllAsync();
    }

    public async Task<IAuthor?> GetAuthorByIdAsync(Guid id)
    {
        _logger.LogInformation("Retrieving author with ID {Id}", id);
        return await _authorRepository.GetByIdAsync(id);
    }

    public async Task<IAuthor> CreateAuthorAsync(IAuthor author)
    {
        _logger.LogInformation("Creating new author with name {Name}", author.Name);
        return await _authorRepository.CreateAsync(author);
    }

    public async Task<IAuthor?> UpdateAuthorAsync(IAuthor author)
    {
        _logger.LogInformation("Updating author with ID {Id}", author.Id);
        return await _authorRepository.UpdateAsync(author);
    }

    public async Task<bool> DeleteAuthorAsync(Guid id)
    {
        _logger.LogInformation("Deleting author with ID {Id}", id);
        return await _authorRepository.DeleteAsync(id);
    }

    public async Task<IAuthor?> GetAuthorByNameAsync(string name)
    {
        _logger.LogInformation("Retrieving author with name {Name}", name);
        return await _authorRepository.GetByNameAsync(name);
    }
}
