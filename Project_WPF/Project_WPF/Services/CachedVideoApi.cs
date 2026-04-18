using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Project_WPF.Models;

namespace Project_WPF.Services
{
    public class CachedVideoApi : IVideoApi
    {
        private readonly IVideoApi _inner;
        private readonly Dictionary<string, CacheItem> _cache = new Dictionary<string, CacheItem>();
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(10);

        public CachedVideoApi(IVideoApi inner)
        {
            _inner = inner;
        }

        public async Task<VideoInfo> GetVideoInfoAsync(string videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                throw new ArgumentException("Video id is empty.");

            CacheItem item;
            if (_cache.TryGetValue(videoId, out item))
            {
                if (DateTime.UtcNow - item.SavedUtc < _ttl)
                    return item.Value;
            }

            var fresh = await _inner.GetVideoInfoAsync(videoId);
            _cache[videoId] = new CacheItem { Value = fresh, SavedUtc = DateTime.UtcNow };
            return fresh;
        }

        private class CacheItem
        {
            public VideoInfo Value;
            public DateTime SavedUtc;
        }
    }
}