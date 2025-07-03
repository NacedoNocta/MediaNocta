using SharedLibrary;
using System.Text.Json;
using ArtLibrary;
using ArtLibrary.Interfaces;
using BlogLibrary;
using BlogLibrary.Interfaces;
using SharedLibrary.Interfaces;

namespace Website.Utils
{
    public static class ActivityJsonReader
    {
        static readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<List<IActivity>> ReadActivitiesAsync(HttpContent httpContent)
        {
            var json = await httpContent.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;
            var activities = new List<IActivity>();

            foreach (var element in root.EnumerateArray())
            {
                var activityType = element.GetProperty("activityType").GetString();

                IActivity activity = activityType switch
                {
                    TextKeys.blogTypeKey => JsonSerializer.Deserialize<Blog>(element.GetRawText(), options) as IActivity,
                    TextKeys.artTypeKey => JsonSerializer.Deserialize<Art>(element.GetRawText(), options) as IActivity,
                    _ => throw new NotSupportedException($"Activity type {activityType} is not supported")
                } ?? throw new NotSupportedException($"Activity not serializable.");

                activities.Add(activity);
            }

            return activities;
        }
    }
}
