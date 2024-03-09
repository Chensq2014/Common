using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Common.Events
{
    /// <summary>
    /// 根据key移除Redis Cache事件
    /// </summary>
    [Serializable]
    public class RemoveCacheEvent: EtoBase//EventData
    {
        public RemoveCacheEvent(string cacheName, string key = "")
        {
            CacheName = cacheName;
            Key = key;
        }

        /// <summary>
        /// 缓存组名
        /// </summary>
        public string CacheName { get; set; }

        /// <summary>
        /// 缓存键Key
        /// </summary>
        public string Key { get; set; }
    }
}
