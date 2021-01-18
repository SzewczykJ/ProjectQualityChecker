using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectQualityChecker.Data;
using ProjectQualityChecker.Data.DataRepository;
using ProjectQualityChecker.Data.IDataRepository;
using ProjectQualityChecker.Services;

namespace ProjectQualityChecker
{
    public class Startup
    {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
                o.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });
           
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseLoggerFactory(MyLoggerFactory); //DEVELOPMENT ONLY
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")).EnableSensitiveDataLogging();
            });

            services.AddHttpClient<SonarQubeClient, SonarQubeClient>(client =>
                client.BaseAddress = new Uri(Configuration["SonarQube_API"]));

            services.AddScoped<SonarQubeScanner, SonarQubeScanner>();
            services.AddScoped<SonarQubeService, SonarQubeService>();
            services.AddScoped<RepositoryService, RepositoryService>();

            services.AddScoped<IBranchRepo, BranchRepo>();
            services.AddScoped<ICommitRepo, CommitRepo>();
            services.AddScoped<IDeveloperRepo, DeveloperRepo>();
            services.AddScoped<IFileRepo, FileRepo>();
            services.AddScoped<IFileDetailRepo, FileDetailRepo>();
            services.AddScoped<ILanguageRepo, LanguageRepo>();
            services.AddScoped<IMetricRepo, MetricsRepo>();
            services.AddScoped<IRepositoryRepo, RepositoryRepo>();

         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            {
                app.UseCors(x =>
                    x.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                );

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                 //   app.UseHsts();
                }

             //   app.UseHttpsRedirection();
             app.UseStaticFiles();

            app.UseRouting();

          //  app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
               endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}