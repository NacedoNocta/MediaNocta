using TechLibrary.Interfaces;

namespace TechAPI.Interfaces
{
    public interface ITechUpdateService
    {
        Task<IEnumerable<ITechUpdate>> GetTechUpdatesAsync(uint page = 1, uint pageSize = 10);
        Task<IEnumerable<ITechUpdate>> GetTechUpdatesByProjectIdAsync(string projectId, uint page = 1, uint pageSize = 10);
        Task<uint> GetTechUpdatesCountAsync();
        Task<uint> GetTechUpdatesCountByProjectIdAsync(string projectId);
        Task<ITechUpdate?> GetTechUpdateByIdAsync(Guid id);
        Task<Dictionary<string, DateTime?>> GetLastActivityDatesAsync();
    }
}