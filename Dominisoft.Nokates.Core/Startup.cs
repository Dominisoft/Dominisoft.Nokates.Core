using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Timers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dominisoft.Nokates.Core
{
    public class Startup
    {
        private static Timer _timer;
        public static string BaseAddressUrl;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SetTimer(15);

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }
        private static void SetTimer(int minutes)
        {
            if (_timer != null) return;

            _timer = new Timer(1000 * 60 * minutes);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
            new HttpClient().GetAsync(BaseAddressUrl);
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var urls = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses;
            BaseAddressUrl = urls.FirstOrDefault();
            Console.WriteLine($"Hosting at {BaseAddressUrl}");
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (!Directory.Exists("../Publish/"))
                Directory.CreateDirectory("../Publish");
        }
    }
}
