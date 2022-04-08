using System;
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
        public static List<ServiceStatus> GetServices(string root)
        {
            var paths = GetApplicationStatusPagePaths(root);
            return paths.Select(path => Task.Run(() => GetStatus(path)).Result).ToList();
        }


        private static async Task<ServiceStatus> GetStatus(string path)
        {
            var name = "";
            try
            {
                var parts = path.Split("/").ToList();
                var index = parts.IndexOf("Nokates") - 1;
                name = parts[index];
                var result = await Get<ServiceStatus>(path);
                result.Uri = path;
                if (result.Name == null)
                    throw new Exception("Failed to get service status");
                return result;
            }
            catch (Exception e)
            {
                StatusValues.EventLog.Add(new LogEntry
                {
                    Date = DateTime.Now,
                    Message = e.Message,
                    Source = "Dominisoft.Nokates.Core"
                });
                Console.WriteLine(e);
            }


            return new ServiceStatus
            {
                IsOnline = false,
                Name = name,
                Uri = path,
            };
        }

        public static Dictionary<string, List<string>> GetGroups(string root)
        {
            var paths = GetApplicationEndpointGroupsPagePaths(root);
            var groups = paths.Select(path => Get<Dictionary<string, List<string>>>(path).Result).ToArray();
            return new Dictionary<string, List<string>>().UnionValues(groups);
        }

        private static async Task<TReturn> Get<TReturn>(string path) where TReturn : new()
        {
            var authClient = new AuthenticationClient("http://localhost/Identity/Authentication");
            var token = await authClient.GetToken("", "");
            return HttpHelper.Get<TReturn>(path, token);
        }

        private static List<string> GetApplicationStatusPagePaths(string root)
        {
            var appPaths = Paths();

            return appPaths.Select(path => $"{root}{path}/Nokates/ServiceStatus").ToList();

        }
        private static List<string> GetApplicationEndpointGroupsPagePaths(string root)
        {
            var appPaths = Paths();

            return appPaths.Select(path => $"{root}{path}/Nokates/EndpointGroups").ToList();

        }
        private static string GetSiteName()
        {
            var serverManager = new ServerManager();
            var sites = serverManager.Sites;
            var maxCount = sites.Select(s => s.Applications.Count).Max();
            var site = sites.FirstOrDefault(s => s.Applications.Count == maxCount);
            return site.Name;

        }
        private static string[] Paths()
        {
            var apps = AppHelper.GetApps();
            var rootName = AppHelper.GetAppName();
            return apps.Where(app => app.ApplicationPoolName !=rootName).Select(app => app.ApplicationPoolName).ToArray();
        }
    }
}
