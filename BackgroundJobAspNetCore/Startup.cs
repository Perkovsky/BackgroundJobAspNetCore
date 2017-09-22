using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Threading;
using Hangfire;
using Hangfire.MemoryStorage;

namespace BackgroundJobAspNetCore
{
    public class Startup
    {
        private static Timer timer;
        private long interval = 5000; // 5 seconds

        private void DoSomething(object obj) => Debug.WriteLine($"Use Timer - {DateTime.Now.ToString()}");
        private void InitTimer() => timer = new Timer(new TimerCallback(DoSomething), null, 0, interval);
        public void DoBackgroundJob() => Debug.WriteLine($"Use Hangfire - {DateTime.Now.ToString()}");

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(p => p.UseMemoryStorage());
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHangfireDashboard(); // Map Dashboard to the `http://<your-app>/hangfire` URL.
            app.UseHangfireServer();
            app.UseMvcWithDefaultRoute();

            BackgroundJob.Enqueue(() => DoBackgroundJob());
            //BackgroundJob.Schedule(() => DoBackgroundJob(), TimeSpan.FromSeconds(5));
            //RecurringJob.AddOrUpdate(() => DoBackgroundJob(), Cron.Minutely());
            //RecurringJob.AddOrUpdate(() => DoBackgroundJob(), "5 * * * *"); // see: https://en.wikipedia.org/wiki/Cron#CRON_expression
            InitTimer();
        }
    }
}
