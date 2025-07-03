using BlogLibrary.Interfaces;

namespace BlogApi.Interfaces;

public interface IBlogRepository
{
    Task<IEnumerable<IBlog>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<IBlog?> GetByIdAsync(Guid id);
    Task<uint> GetCountAsync();
    Task<IBlog> CreateAsync(IBlog blog);
    Task<IBlog?> UpdateAsync(IBlog blog);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<IBlog>> GetByAuthorIdAsync(Guid authorId);
    Task<IEnumerable<IBlog>> GetByTagIdAsync(Guid tagId);
}