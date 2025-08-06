namespace Website.Services
{
    /// <summary>
    /// HTTP client interface that automatically injects JWT tokens from the current session
    /// </summary>
    public interface IAuthenticatedHttpClient
    {
        /// <summary>
        /// Send a GET request to the specified URI
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string requestUri);

        /// <summary>
        /// Send a GET request to the specified URI with cancellation token
        /// </summary>
        Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken);

        /// <summary>
        /// Send a POST request to the specified URI
        /// </summary>
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content);

        /// <summary>
        /// Send a POST request to the specified URI with cancellation token
        /// </summary>
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken);

        /// <summary>
        /// Send a PUT request to the specified URI
        /// </summary>
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent? content);

        /// <summary>
        /// Send a PUT request to the specified URI with cancellation token
        /// </summary>
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken);

        /// <summary>
        /// Send a DELETE request to the specified URI
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(string requestUri);

        /// <summary>
        /// Send a DELETE request to the specified URI with cancellation token
        /// </summary>
        Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken);

        /// <summary>
        /// Send a PATCH request to the specified URI
        /// </summary>
        Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent? content);

        /// <summary>
        /// Send a PATCH request to the specified URI with cancellation token
        /// </summary>
        Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken);

        /// <summary>
        /// Send an HTTP request
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        /// <summary>
        /// Send an HTTP request with cancellation token
        /// </summary>
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}