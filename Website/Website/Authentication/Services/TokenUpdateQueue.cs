using System.Collections.Concurrent;

namespace Website.Authentication.Services;

public class TokenUpdateQueue
{
    private readonly ConcurrentQueue<TokenUpdateRequest> _updateQueue = new();
    
    public void QueueTokenUpdate(string userId, string accessToken, string refreshToken, string? idToken = null)
    {
        // Remove any existing updates for this user
        var tempQueue = new ConcurrentQueue<TokenUpdateRequest>();
        while (_updateQueue.TryDequeue(out var existing))
        {
            if (existing.UserId != userId)
            {
                tempQueue.Enqueue(existing);
            }
        }
        
        // Put back the non-matching items
        while (tempQueue.TryDequeue(out var item))
        {
            _updateQueue.Enqueue(item);
        }
        
        // Add the new update
        _updateQueue.Enqueue(new TokenUpdateRequest
        {
            UserId = userId,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IdToken = idToken,
            Timestamp = DateTime.UtcNow
        });
    }
    
    public TokenUpdateRequest? DequeueTokenUpdate()
    {
        if (_updateQueue.TryDequeue(out var request))
        {
            // Skip old requests (older than 5 minutes)
            if (DateTime.UtcNow - request.Timestamp > TimeSpan.FromMinutes(5))
            {
                return DequeueTokenUpdate(); // Try next item
            }
            return request;
        }
        return null;
    }
    
    public bool HasPendingUpdates => !_updateQueue.IsEmpty;
    
    public void ClearUpdatesForUser(string userId)
    {
        var tempQueue = new ConcurrentQueue<TokenUpdateRequest>();
        while (_updateQueue.TryDequeue(out var existing))
        {
            if (existing.UserId != userId)
            {
                tempQueue.Enqueue(existing);
            }
        }
        
        // Put back the non-matching items
        while (tempQueue.TryDequeue(out var item))
        {
            _updateQueue.Enqueue(item);
        }
    }
    
    public class TokenUpdateRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string? IdToken { get; set; }
        public DateTime Timestamp { get; set; }
    }
}