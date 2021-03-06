using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace quickstartcore31
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
            services.AddControllersWithViews();
            var useInMemoryDb = Configuration["UseInMemoryDb"];
            var useAzureDocumentClient = Configuration["UseAzureDocumentClient"];
            if (useAzureDocumentClient == "true")
            {
                services.AddSingleton<IDocumentDBRepository<Models.Item>>(new DocumentDBRepository<Models.Item>(Configuration));
            }
            else if (useInMemoryDb == "true")
            {
                services.AddDbContext<TodoDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TodoList");                    
                });
                services.AddScoped<IDocumentDBRepository<Models.Item>, CosmoDBEFRepository<Models.Item>>();
            }
            else
            {

                services.AddDbContext<TodoDbContext>(options =>
                {
                    options.UseLoggerFactory(TodoDbContext.LoggerFactory);                    
                    options.UseCosmos(
                        accountEndpoint: Configuration["CosmosDbInfo:endpoint"],
                        accountKey: Configuration["CosmosDbInfo:authKey"],
                        databaseName: Configuration["CosmosDbInfo:database"]);
                    options.EnableSensitiveDataLogging();
                    
                    
                });
                services.AddScoped<IDocumentDBRepository<Models.Item>, CosmoDBEFRepository<Models.Item>>();
            }
            
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Item}/{action=Index}/{id?}");
            });


        }
    }
}
