using System.Net.Http;
using AutoMapper;
using Hangfire;
using Hangfire.Mongo;
using mars_marking_svc.BackgroundJobs;
using mars_marking_svc.BackgroundJobs.Interfaces;
using mars_marking_svc.DependantResource;
using mars_marking_svc.DependantResource.Interfaces;
using mars_marking_svc.MarkSession.Interfaces;
using mars_marking_svc.Middlewares;
using mars_marking_svc.ResourceTypes.MarkedResource;
using mars_marking_svc.ResourceTypes.Metadata;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.Project;
using mars_marking_svc.ResourceTypes.Project.Interfaces;
using mars_marking_svc.ResourceTypes.ResultConfig;
using mars_marking_svc.ResourceTypes.ResultConfig.Interfaces;
using mars_marking_svc.ResourceTypes.ResultData;
using mars_marking_svc.ResourceTypes.ResultData.Interfaces;
using mars_marking_svc.ResourceTypes.Scenario;
using mars_marking_svc.ResourceTypes.Scenario.Interfaces;
using mars_marking_svc.ResourceTypes.SimPlan;
using mars_marking_svc.ResourceTypes.SimPlan.Interfaces;
using mars_marking_svc.ResourceTypes.SimRun;
using mars_marking_svc.ResourceTypes.SimRun.Interfaces;
using mars_marking_svc.Services;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace mars_marking_svc
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
            services.AddMvc();

            // AutoMapper config
            services.AddAutoMapper();

            // Hangfire config
            services.AddHangfire(configuration =>
            {
                configuration.UseMongoStorage(
                    DbMongoService.MongoDbConnection,
                    DbMongoService.MongoDbHangfireName
                );
            });

            // Services
            services.AddScoped<IDbMongoService, DbMongoService>();
            services.AddScoped<IHostedService, HostedStartupService>();
            services.AddScoped<IHttpService, HttpService>();
            services.AddScoped<ILoggerService, LoggerService>();

            // Clients
            services.AddSingleton<HttpClient>();
            services.AddScoped<IProjectClient, ProjectClient>();
            services.AddScoped<IMetadataClient, MetadataClient>();
            services.AddScoped<IScenarioClient, ScenarioClient>();
            services.AddScoped<IResultConfigClient, ResultConfigClient>();
            services.AddScoped<ISimPlanClient, SimPlanClient>();
            services.AddScoped<ISimRunClient, SimRunClient>();
            services.AddScoped<IResultDataClient, ResultDataClient>();
            services.AddScoped<IMarkSessionRepository, MarkSessionRepository>();

            // Handlers
            services.AddScoped<IMarkSessionHandler, MarkSessionHandler>();
            services.AddScoped<IDependantResourceHandler, DependantResourceHandler>();
            services.AddScoped<IBackgroundJobsHandler, BackgroundJobsHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Must always be on top !!!
            app.UseMiddleware<ErrorHandlerMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // Hangfire config
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}