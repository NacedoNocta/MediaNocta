using SharedLibrary;

namespace SharedLibrary.Interfaces;

public interface IActivityRepository
{
    Task<IEnumerable<Activity>> GetRecentActivitiesAsync(int count = 5);
    Task<IEnumerable<Activity>> GetPinnedActivitiesAsync(int count = 2);
    Task<IEnumerable<Activity>> GetRandomActivitiesAsync(int count = 1);
}