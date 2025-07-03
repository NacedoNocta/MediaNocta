using SharedLibrary;
using SharedLibrary.Interfaces;
using Website.Utils;

namespace Website.Services
{
    public class ActivityService
    {
        HttpClient httpClient;
        public ActivityService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<List<IActivity>> GetRecent()
        {
            List<IActivity>? recent = null;
            var response = await httpClient.GetAsync("recent");
            if (response.IsSuccessStatusCode)
            {
                recent = await ActivityJsonReader.ReadActivitiesAsync(response.Content);
            }

            return recent ?? new List<IActivity>();
        }
        public async Task<List<IActivity>> GetPinned()
        {
            List<IActivity>? pinned = null;
            var response = await httpClient.GetAsync("pinned");
            if (response.IsSuccessStatusCode)
            {
                pinned = await ActivityJsonReader.ReadActivitiesAsync(response.Content);
            }

            return pinned ?? new List<IActivity>();
        }
    }
}
