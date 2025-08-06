using TechAPI.Interfaces;
using TechLibrary;
using TechLibrary.Interfaces;
using DatabaseManager;
using Microsoft.EntityFrameworkCore;

namespace TechAPI.Repositories
{
    public class EfTechUpdateRepository : ITechUpdateRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EfTechUpdateRepository> _logger;

        public EfTechUpdateRepository(AppDbContext context, ILogger<EfTechUpdateRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<ITechUpdate>> GetTechUpdatesAsync(uint page = 1, uint pageSize = 10)
        {
            try
            {
                var skip = (int)((page - 1) * pageSize);
                return await _context.TechUpdates
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip(skip)
                    .Take((int)pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates for page {Page}", page);
                throw;
            }
        }

        public async Task<IEnumerable<ITechUpdate>> GetTechUpdatesByProjectIdAsync(string projectId, uint page = 1, uint pageSize = 10)
        {
            try
            {
                var skip = (int)((page - 1) * pageSize);
                return await _context.TechUpdates
                    .Where(t => t.ProjectId == projectId)
                    .OrderByDescending(t => t.CreatedAt)
                    .Skip(skip)
                    .Take((int)pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates for project {ProjectId}, page {Page}", projectId, page);
                throw;
            }
        }

        public async Task<uint> GetTechUpdatesCountAsync()
        {
            try
            {
                return (uint)await _context.TechUpdates.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates count");
                throw;
            }
        }

        public async Task<uint> GetTechUpdatesCountByProjectIdAsync(string projectId)
        {
            try
            {
                return (uint)await _context.TechUpdates
                    .Where(t => t.ProjectId == projectId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech updates count for project {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<ITechUpdate?> GetTechUpdateByIdAsync(Guid id)
        {
            try
            {
                return await _context.TechUpdates
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tech update with ID {Id}", id);
                throw;
            }
        }

        public async Task<Dictionary<string, DateTime?>> GetLastActivityDatesAsync()
        {
            try
            {
                return await _context.TechUpdates
                    .GroupBy(t => t.ProjectId)
                    .Select(g => new { ProjectId = g.Key, LastActivity = g.Max(t => t.CreatedAt) })
                    .ToDictionaryAsync(x => x.ProjectId, x => (DateTime?)x.LastActivity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving last activity dates");
                throw;
            }
        }

        public async Task<ITechUpdate> CreateTechUpdateAsync(ITechUpdate techUpdate)
        {
            try
            {
                var sourceEntity = techUpdate as TechUpdate ?? throw new ArgumentException("Tech update must be of type TechUpdate", nameof(techUpdate));
                
                var entity = new TechUpdate(
                    sourceEntity.Title,
                    sourceEntity.Summary, 
                    sourceEntity.Content,
                    sourceEntity.ProjectId,
                    sourceEntity.UpdateType,
                    sourceEntity.ImageUrl,
                    sourceEntity.Links?.Count > 0 ? sourceEntity.Links : null
                );

                _context.TechUpdates.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created tech update with ID {Id}", entity.Id);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tech update");
                throw;
            }
        }

        public async Task<ITechUpdate?> UpdateTechUpdateAsync(ITechUpdate techUpdate)
        {
            try
            {
                var entity = techUpdate as TechUpdate ?? throw new ArgumentException("Tech update must be of type TechUpdate", nameof(techUpdate));
                
                var existingEntity = await _context.TechUpdates.FirstOrDefaultAsync(t => t.Id == entity.Id);
                if (existingEntity == null)
                {
                    _logger.LogWarning("Tech update with ID {Id} not found for update", entity.Id);
                    return null;
                }

                existingEntity.Title = entity.Title;
                existingEntity.Summary = entity.Summary;
                existingEntity.Content = entity.Content;
                existingEntity.ProjectId = entity.ProjectId;
                existingEntity.UpdateType = entity.UpdateType;
                existingEntity.ImageUrl = entity.ImageUrl;
                existingEntity.Links = entity.Links;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated tech update with ID {Id}", entity.Id);
                return existingEntity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tech update with ID {Id}", techUpdate.Id);
                throw;
            }
        }

        public async Task<bool> DeleteTechUpdateAsync(Guid id)
        {
            try
            {
                var entity = await _context.TechUpdates.FirstOrDefaultAsync(t => t.Id == id);
                if (entity == null)
                {
                    _logger.LogWarning("Tech update with ID {Id} not found for deletion", id);
                    return false;
                }

                _context.TechUpdates.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted tech update with ID {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tech update with ID {Id}", id);
                throw;
            }
        }
    }
}