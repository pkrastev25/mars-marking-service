using System.Net.Http;
using mars_marking_svc.ResourceTypes.Metadata;
using mars_marking_svc.ResourceTypes.Metadata.Models;
using mars_marking_svc.ResourceTypes.Scenario;
using mars_marking_svc.ResourceTypes.Scenario.Models;
using mars_marking_svc.Services;
using mars_marking_svc.Services.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

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
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    // TODO: Figure out why this does nothing!
                    options.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                });

            services.AddSingleton<HttpClient>();
            services.AddTransient<IHttpService, HttpService>();
            services.AddTransient<ILoggerService, LoggerService>();
            services.AddTransient<IMetadataServiceClient, MetadataServiceClient>();
            services.AddTransient<IMetadataResourceHandlerService, MetadataResourceHandlerService>();
            services.AddTransient<IScenarioServiceClient, ScenarioServiceClient>();
            services.AddTransient<IScenarioResourceHandlerService, ScenarioResourceHandlerService>();
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