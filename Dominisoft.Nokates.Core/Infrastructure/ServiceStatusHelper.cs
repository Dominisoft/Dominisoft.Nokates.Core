﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominisoft.Nokates.Common.Infrastructure.Client;
using Dominisoft.Nokates.Common.Infrastructure.Configuration;
using Dominisoft.Nokates.Common.Infrastructure.Helpers;
using Dominisoft.Nokates.Common.Models;
using Microsoft.Web.Administration;

namespace Dominisoft.Nokates.Core.Infrastructure
{
    public static class ServiceStatusHelper
    {
        public static string SiteName;
        private static readonly Dictionary<string, CacheItem> _cachedStatuses = new Dictionary<string, CacheItem>();
        public static List<ServiceStatus> GetServices(string root)
        {
            var paths = GetApplicationStatusPagePaths(root);
            return paths.Select(GetStatus).ToList();
        }

        public static ServiceStatus GetStatus(string path)
            => GetStatus(path,true);

        public static ServiceStatus GetStatus(string path, bool useCache)
        {
            var name = "";
            try
            {
                if(useCache)
                if (_cachedStatuses.ContainsKey(path))
                {
                    var cachedItem = _cachedStatuses[path];
                  //  if (cachedItem.CachedTime > DateTime.Now.AddMinutes(-5) && cachedItem.CachedTime < DateTime.Now.AddSeconds(5))
                        return cachedItem.Status;
                }

                //var token = ConfigurationValues.Token;
                //StatusValues.EventLog.Add(new LogEntry
                //{
                //    Date = DateTime.Now,
                //    Message = $"Got Token: {token?.Split('.').Length==3}",
                //    Source = "Dominisoft.Nokates.Core"
                //});
                
                var parts = path.Split("/").ToList();
                var index = parts.IndexOf("Nokates") - 1;
                name = parts[index];
                var result = Get<ServiceStatus>(path);
                result.Uri = path;
                if (result.Name == null)
                    throw new Exception("Failed to get service status");
                if (!useCache) return result;

                if (!_cachedStatuses.ContainsKey(path))
                {
                    var cacheItem = new CacheItem(result);
                    cacheItem.ItemExpired += (s, e) =>
                    {
                        var item = (CacheItem)s;
                        var path = item.Path;
                        _cachedStatuses.Remove(path);

                    };
                }

                return result;
            }
            catch (Exception e)
            {
                StatusValues.EventLog.Add(new LogEntry
                {
                    Date = DateTime.Now,
                    Message = e.Message+"\r\n"+e.StackTrace,
                    Source = "Dominisoft.Nokates.Core"
                });
                Console.WriteLine(e);
            }
            var offlineResult = new ServiceStatus
            {
                IsOnline = false,
                Name = name,
                Uri = path,
            };

            return offlineResult;
        }

        public static Dictionary<string, List<string>> GetGroups(string root)
        {
            var paths = GetApplicationEndpointGroupsPagePaths(root);
            var groups = paths.Select(Get<Dictionary<string, List<string>>>).ToArray();
            return new Dictionary<string, List<string>>().UnionValues(groups);
        }

        private static TReturn Get<TReturn>(string path) where TReturn : class,new() 
            => HttpHelper.Get<TReturn>(path,ConfigurationValues.Token).Object??new TReturn();

        internal static List<string> GetApplicationStatusPagePaths(string root)
        {
            var appPaths = GetServicePaths();

            return appPaths.Select(path => $"{root}{path}/Nokates/ServiceStatus").ToList();

        }
        internal static List<string> GetApplicationEndpointGroupsPagePaths(string root)
        {
            var appPaths = GetServicePaths();

            return appPaths.Select(path => $"{root}{path}/Nokates/EndpointGroups").ToList();

        }
        private static string GetSiteName()
        {
            var serverManager = new ServerManager();
            var sites = serverManager.Sites;
            var maxCount = sites.Select(s => s.Applications.Count).Max();
            var site = sites.FirstOrDefault(s => s.Applications.Count == maxCount);
            return site?.Name;

        }
        internal static string[] GetServicePaths()
        {
            var apps = AppHelper.GetApps();
            var rootName = AppHelper.GetAppName();
            return apps.Where(app => app.ApplicationPoolName !=rootName).Select(app => app.ApplicationPoolName).ToArray();
        }
    }
}
