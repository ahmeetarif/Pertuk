using System;
using System.Threading.Tasks;

namespace Pertuk.Business.Services.Abstract
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeLive);
        Task<string> GetCachesResponseAsync(string cacheKey);
    }
}