using Newtonsoft.Json;
using PaymentSystem.Application.Constants.Services.Abstract;
using StackExchange.Redis;

namespace PaymentSystem.Application.Constants.Caching.Redis
{
    public class RedisManager : ICacheService
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connection;

        public RedisManager(IConnectionMultiplexer connection)
        {
            _connection = connection;
            _database = connection.GetDatabase();
        }

        public T? Get<T>(string key)
        {
            var value = _database.StringGet(key);
            if (value.IsNullOrEmpty) return default;
            return JsonConvert.DeserializeObject<T>(value);
        }

        public void Set<T>(string key, T value, TimeSpan expirationTime)
        {
            var json = JsonConvert.SerializeObject(value);
            _database.StringSet(key, json, expirationTime);
        }

        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }

        public bool Any(string key)
        {
            return _database.KeyExists(key);
        }
    }
}
