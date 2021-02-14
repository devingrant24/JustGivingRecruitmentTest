using JG.FinTechTest.Handlers;
using JG.FinTechTest.Handlers.Interfaces;
using JG.FinTechTest.Helpers;
using JG.FinTechTest.Helpers.Interfaces;
using JG.FinTechTest.Models.Options;
using JG.FinTechTest.Storage;
using JG.FinTechTest.Storage.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace JG.FinTechTest
{
    public class Startup
    {
        private const string GiftAidOptionsKey = "GiftAid";
        private const string StorageKey = "Storage";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver()); // To ensure properties are serialized in camelcase to match the swagger spec.

            services.AddScoped<IGiftAidHandler, GiftAidHandler>();

            services.AddSingleton<IGiftAidCalculator, GiftAidCalculator>();
            services.Configure<GiftAidOptions>(Configuration.GetSection(GiftAidOptionsKey));

            services.AddScoped<IGiftAidDeclarationRepository, GiftAidDeclarationRepository>();
            services.Configure<StorageOptions>(Configuration.GetSection(StorageKey));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
