using System;
using System.Threading;
using Dominisoft.Nokates.Common.Models;

namespace Dominisoft.Nokates.Core
{
    public class CacheItem
    {
        public event EventHandler ItemExpired; 
        public CacheItem(ServiceStatus status)
        {
            CachedTime = DateTime.Now;
            Status = status;
            var dueTime = new TimeSpan(0, 5, 0);
            refreshTimer = new Timer(OnExpired,null,dueTime: dueTime,new TimeSpan(0,0,0,0,-1));
            
        }

        private void OnExpired(object state)
        {
            ItemExpired?.Invoke(this, EventArgs.Empty);
        }

        public string Path => Status?.Uri;
        public DateTime CachedTime { get; set; }
        public ServiceStatus Status { get; set; }
        private Timer refreshTimer;
    }
}
