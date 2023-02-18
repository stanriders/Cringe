using System;
using Cringe.Database;
using Cringe.Mappings;
using Cringe.Services;
using Cringe.Web.Mappings;
using Cringe.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
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
            services.AddTransient<BanchoApiWrapper>();
            services.AddTransient<BeatconnectApiWrapper>();
            services.AddTransient<PlayerTopscoreStatsCache>();
            services.AddTransient<PlayerRankRetriever>();
            services.AddTransient<ReplayStorage>();
            services.AddTransient<ScoreService>();
            services.AddTransient<FlagsService>();

            services.AddHttpClient<OsuApiWrapper>();
            services.AddHttpClient<BanchoApiWrapper>(client =>
            {
                client.BaseAddress = new Uri(Configuration["BanchoApiAddress"] ?? "http://127.0.0.1");
            });
            services.AddHttpClient<BeatconnectApiWrapper>(client =>
            {
                client.BaseAddress = new Uri("https://beatconnect.io/api/");
            });

            services.AddMemoryCache();

            services.AddAutoMapper(typeof(MappingProfile), typeof(FrontendMappingProfile));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddControllers();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Account");
                options.Conventions.AllowAnonymousToPage("/Account/Login");
            });
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
