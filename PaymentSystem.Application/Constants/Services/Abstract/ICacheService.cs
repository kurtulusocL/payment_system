
namespace PaymentSystem.Application.Constants.Services.Abstract
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expirationTime);
        void Remove(string key);
        bool Any(string key);
    }
}
