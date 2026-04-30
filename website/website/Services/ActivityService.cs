using SharedLibrary;
using SharedLibrary.Interfaces;
using Website.Utils;

namespace Website.Services
{
    public class ActivityService
    {
        private static readonly TimeSpan ListTtl = TimeSpan.FromDays(1);
        HttpClient httpClient;
        public ActivityService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<List<IActivity>> GetRecent()
        {
            List<IActivity>? recent = null;
            using var request = new HttpRequestMessage(HttpMethod.Get, "recent");
            request.Options.Set(WebsiteCachingHandler.CacheableTtl, ListTtl);
            using var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                recent = await ActivityJsonReader.ReadActivitiesAsync(response.Content);
            }

            return recent ?? new List<IActivity>();
        }
        public async Task<List<IActivity>> GetPinned()
        {
            List<IActivity>? pinned = null;
            using var request = new HttpRequestMessage(HttpMethod.Get, "pinned");
            request.Options.Set(WebsiteCachingHandler.CacheableTtl, ListTtl);
            using var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                pinned = await ActivityJsonReader.ReadActivitiesAsync(response.Content);
            }

            return pinned ?? new List<IActivity>();
        }
    }
}
