using BlogLibrairy;
using System.Text.Json;


namespace website.Services
{
    public class BlogService
    {

        HttpClient httpClient;
        public BlogService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<List<Blog>> GetPosts()
        {
            List<Blog>? posts = null;
            var response = await httpClient.GetAsync("/SimpleBlog");
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                posts = await response.Content.ReadFromJsonAsync(BlogLibrairyJsonContext.Default.ListBlog);
            }

            return posts ?? new List<Blog>();
        }

    }
}
