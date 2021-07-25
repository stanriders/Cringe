using Cringe.Database;
using Cringe.Services;
using Cringe.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cringe.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PlayerDatabaseContext>();
            services.AddDbContext<ScoreDatabaseContext>();
            services.AddDbContext<BeatmapDatabaseContext>();
            services.AddTransient<BeatmapService>();
            services.AddTransient<PpService>();
            services.AddTransient<OsuApiWrapper>();
            services.AddTransient<PlayerTopscoreStatsCache>();
            services.AddTransient<PlayerRankCache>();
            services.AddTransient<ReplayStorage>();
            services.AddTransient<ScoreService>();

            services.AddHttpClient<OsuApiWrapper>();
            services.AddMemoryCache();

            services.AddControllers();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
