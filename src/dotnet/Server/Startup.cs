using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Server.Game;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration
        {
            get;
        }

        private void RegisterGameRelatedDependencies(IServiceCollection services)
        {
            services.AddHostedService<GameServerHostedService>();

            services.AddSingleton(p => 
                new GameServer("127.0.0.1", 14567));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddCors(options =>
            {
                options.AddPolicy("CORS", builder => builder
                    .WithOrigins(
                        "http://localhost:5000",
                        "http://localhost:14568")
                    .WithHeaders(
                        "Content-Type")
                    .WithMethods(
                        "POST",
                        "GET",
                        "PUT",
                        "PATCH"));
            });

            RegisterGameRelatedDependencies(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("CORS");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
