using Website.Authentication.Interfaces;

namespace Website.Services
{
    /// <summary>
    /// HTTP client that automatically injects JWT tokens from the current session
    /// </summary>
    public class AuthenticatedHttpClient : IAuthenticatedHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger<AuthenticatedHttpClient> _logger;

        public AuthenticatedHttpClient(
            HttpClient httpClient,
            ISessionManager sessionManager,
            ILogger<AuthenticatedHttpClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionManager = sessionManager ?? throw new ArgumentNullException(nameof(sessionManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await GetAsync(requestUri, CancellationToken.None);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content)
        {
            return await PostAsync(requestUri, content, CancellationToken.None);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri) { Content = content };
            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent? content)
        {
            return await PutAsync(requestUri, content, CancellationToken.None);
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri) { Content = content };
            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return await DeleteAsync(requestUri, CancellationToken.None);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent? content)
        {
            return await PatchAsync(requestUri, content, CancellationToken.None);
        }

        public async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent? content, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, requestUri) { Content = content };
            return await SendAsync(request, cancellationToken);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return await SendAsync(request, CancellationToken.None);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await AddAuthenticationHeaderAsync(request);
            return await _httpClient.SendAsync(request, cancellationToken);
        }

        private async Task AddAuthenticationHeaderAsync(HttpRequestMessage request)
        {
            try
            {
                var accessToken = await _sessionManager.GetValidAccessTokenAsync();
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    _logger.LogDebug("Added Bearer token to request: {Method} {Uri}", request.Method, request.RequestUri);
                }
                else
                {
                    _logger.LogDebug("No valid access token available for request: {Method} {Uri}", request.Method, request.RequestUri);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add authentication header to request: {Method} {Uri}", request.Method, request.RequestUri);
            }
        }
    }
}