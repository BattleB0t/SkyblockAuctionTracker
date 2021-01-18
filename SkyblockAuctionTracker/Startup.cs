using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Pomelo.EntityFrameworkCore.MySql.Storage;
using Quartz;
using SkyblockAuctionTracker.ApiServices;
using SkyblockAuctionTracker.Models;
using SkyblockAuctionTracker.Schedules;
using System;
using System.Net.Http;

namespace SkyblockAuctionTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
                .AddJsonFile("keys.json");
            KeysConfiguration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        public IConfiguration KeysConfiguration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<SkyblockApiServiceOptions>().Configure(o => o
                .ApiToken = KeysConfiguration.GetSection("ApiKeys").GetSection("SkyblockApiKey").Value);
            services.AddHttpClient<ISkyblockApiService, SkyblockApiService>();

            // .NET CORE 5.0
            /*
            services.AddDbContext<AMCDbContext>(options => 
                options.UseMySql(Configuration.GetConnectionString("SkyblockAuctionTrackerMariaDB"),
                    new MariaDbServerVersion(new Version(10, 4, 6))
            ));
            */
            // .NET CORE 3.1
            services.AddDbContext<AMCDbContext>(options =>
                 options.UseMySql(KeysConfiguration.GetConnectionString("SkyblockAuctionTrackerMariaDB"),
                    o => o.ServerVersion(new ServerVersion(new Version(10, 4, 6), ServerType.MariaDb))
            ));

            services.Configure<QuartzOptions>(Configuration.GetSection("Quartz"));
            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler-Core";

                q.UseMicrosoftDependencyInjectionScopedJobFactory(c => c.CreateScope = true);

                // Defaults
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp =>
                {
                    tp.MaxConcurrency = 10;
                });

                q.ScheduleJob<BazaarScheduleJob>(trigger => trigger
                    .WithIdentity("BazaarScheduleTrigger")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromMinutes(1))
                        .RepeatForever())
                    .WithDescription("BazaarScheduleTriggerDesc")
                );
            });

            // ASP.NET Core hosting
            services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SkyblockAuctionTracker", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkyblockAuctionTracker v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
