using System.Net.Http;
using mars_marking_svc.ResourceTypes.MarkedResource;
using mars_marking_svc.ResourceTypes.MarkedResource.Interfaces;
using mars_marking_svc.ResourceTypes.Metadata;
using mars_marking_svc.ResourceTypes.Metadata.Interfaces;
using mars_marking_svc.ResourceTypes.ProjectContents;
using mars_marking_svc.ResourceTypes.ProjectContents.Interfaces;
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
            services.AddSingleton<HttpClient>();
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<ILoggerService, LoggerService>();
            services.AddTransient<IMetadataServiceClient, MetadataServiceClient>();
            services.AddTransient<IMetadataResourceHandler, MetadataResourceHandler>();
            services.AddTransient<IScenarioServiceClient, ScenarioServiceClient>();
            services.AddTransient<IScenarioResourceHandler, ScenarioResourceHandler>();
            services.AddTransient<IResultConfigServiceClient, ResultConfigServiceClient>();
            services.AddTransient<IResultConfigResourceHandler, ResultConfigResourceHandler>();
            services.AddTransient<ISimPlanServiceClient, SimPlanServiceClient>();
            services.AddTransient<ISimPlanResourceHandler, SimPlanResourceHandler>();
            services.AddTransient<ISimRunServiceClient, SimRunServiceClient>();
            services.AddTransient<ISimRunResourceHandler, SimRunResourceHandler>();
            services.AddTransient<IProjectResourceHandler, ProjectResourceHandler>();
            services.AddTransient<IResultDataServiceClient, ResultDataServiceClient>();
            services.AddSingleton<IDbService, DbService>();
            services.AddTransient<IDbMarkSessionHandler, DbMarkSessionHandler>();
            services.AddTransient<IErrorHandlerService, ErrorHandlerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}