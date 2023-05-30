using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Drugi_Projekat
{
    internal class Cache
    {
        private MemoryCache _cache;
        private DateTime _accessTime;
        public Cache(string name)
        {
            _cache = new MemoryCache(name);
            _accessTime = DateTime.Now;
        }

        public bool Contains(string filePath)
        {
            return _cache.Contains(filePath);
        }
        public int GetValue(string filePath)
        {
            return (int)_cache.Get(filePath);
        }

        public void Set(string filePath, int count)
        {   
            if(!this.Contains(filePath))
                _cache.Set(filePath, count, DateTimeOffset.UtcNow.AddHours(1));
            else if(this.GetValue(filePath) != count)
                _cache.Set(filePath, count, DateTimeOffset.UtcNow.AddHours(1));
        }
        public DateTime AccessTime
        {
            get { return _accessTime; }
            set { if(value != _accessTime) _accessTime = value; }
        }
    }
}
