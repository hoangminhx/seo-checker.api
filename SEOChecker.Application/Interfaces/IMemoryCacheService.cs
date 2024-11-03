namespace SEOChecker.Application.Interfaces
{
    public interface IMemoryCacheService
    {
        bool TryGetValue<T>(string key, out T? value);
        void Set<T> (string key, T value, TimeSpan expiration);
    }
}
