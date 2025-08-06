using SharedLibrary;
using SharedLibrary.Interfaces;
using Website.Utils;

namespace Website.Services
{
    public interface IActivityService
    {
        Task<List<IActivity>> GetRecent();
        Task<List<IActivity>> GetPinned();
    }

    public class ActivityService : IActivityService
    {
        private readonly IAuthenticatedHttpClient _authenticatedHttpClient;
        private const string ApiEndpoint = "/api/activity";
        
        
        public ActivityService(IAuthenticatedHttpClient authenticatedHttpClient)
        {
            _authenticatedHttpClient = authenticatedHttpClient ?? throw new ArgumentNullException(nameof(authenticatedHttpClient));
        }
        public async Task<List<IActivity>> GetRecent()
        {
            List<IActivity>? recent = null;
            var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}/recent");
            if (response.IsSuccessStatusCode)
            {
                recent = await ActivityJsonReader.ReadActivitiesAsync(response.Content);
            }

            return recent ?? new List<IActivity>();
        }
        
        public async Task<List<IActivity>> GetPinned()
        {
            List<IActivity>? pinned = null;
            var response = await _authenticatedHttpClient.GetAsync($"{ApiEndpoint}/pinned");
            if (response.IsSuccessStatusCode)
            {
                pinned = await ActivityJsonReader.ReadActivitiesAsync(response.Content);
            }

            return pinned ?? new List<IActivity>();
        }
    }
}
