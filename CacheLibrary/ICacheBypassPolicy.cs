namespace CacheLibrary;

public interface ICacheBypassPolicy
{
    bool ShouldBypass();
}
