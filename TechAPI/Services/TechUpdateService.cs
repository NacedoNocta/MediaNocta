using TechAPI.Interfaces;
using TechLibrary.Interfaces;

namespace TechAPI.Services
{
    public class TechUpdateService : ITechUpdateService
    {
        private readonly ITechUpdateRepository _repository;
        private readonly ILogger<TechUpdateService> _logger;

        public TechUpdateService(ITechUpdateRepository repository, ILogger<TechUpdateService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<ITechUpdate>> GetTechUpdatesAsync(uint page = 1, uint pageSize = 10)
        {
            try
            {
                return await _repository.GetTechUpdatesAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service retrieving tech updates for page {Page}", page);
                throw;
            }
        }

        public async Task<IEnumerable<ITechUpdate>> GetTechUpdatesByProjectIdAsync(string projectId, uint page = 1, uint pageSize = 10)
        {
            try
            {
                return await _repository.GetTechUpdatesByProjectIdAsync(projectId, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service retrieving tech updates for project {ProjectId}, page {Page}", projectId, page);
                throw;
            }
        }

        public async Task<uint> GetTechUpdatesCountAsync()
        {
            try
            {
                return await _repository.GetTechUpdatesCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service retrieving tech updates count");
                throw;
            }
        }

        public async Task<uint> GetTechUpdatesCountByProjectIdAsync(string projectId)
        {
            try
            {
                return await _repository.GetTechUpdatesCountByProjectIdAsync(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service retrieving tech updates count for project {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<ITechUpdate?> GetTechUpdateByIdAsync(Guid id)
        {
            try
            {
                return await _repository.GetTechUpdateByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service retrieving tech update with ID {Id}", id);
                throw;
            }
        }

        public async Task<Dictionary<string, DateTime?>> GetLastActivityDatesAsync()
        {
            try
            {
                return await _repository.GetLastActivityDatesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service retrieving last activity dates");
                throw;
            }
        }

        public async Task<ITechUpdate> CreateTechUpdateAsync(ITechUpdate techUpdate)
        {
            try
            {
                return await _repository.CreateTechUpdateAsync(techUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service creating tech update");
                throw;
            }
        }

        public async Task<ITechUpdate?> UpdateTechUpdateAsync(ITechUpdate techUpdate)
        {
            try
            {
                return await _repository.UpdateTechUpdateAsync(techUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service updating tech update with ID {Id}", techUpdate.Id);
                throw;
            }
        }

        public async Task<bool> DeleteTechUpdateAsync(Guid id)
        {
            try
            {
                return await _repository.DeleteTechUpdateAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in service deleting tech update with ID {Id}", id);
                throw;
            }
        }
    }
}