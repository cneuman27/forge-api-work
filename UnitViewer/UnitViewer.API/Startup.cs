using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

using JSONFormatters;
using ModelAccess.AspNet;
using ForgeAPI.AspNet;

namespace UnitViewer.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        public IHostingEnvironment Environment { get; private set; }

        public Startup(
            IConfiguration configuration,
            IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors(
                    options =>
                    {
                        options.AddPolicy("AllowAll",
                            builder =>
                            {
                                builder
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowAnyOrigin();
                            });
                    })
                .AddMvc(
                    options =>
                    {
                        options.RespectBrowserAcceptHeader = true;
                    })
                .AddExpandedModelJSONFormatters()
                .AddMinifiedModelJSONFormatters()
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                        options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
                        options.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;

                        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

                        if (options.SerializerSettings.ContractResolver != null)
                        {
                            DefaultContractResolver resolver = options.SerializerSettings.ContractResolver as DefaultContractResolver;

                            resolver.NamingStrategy = null;
                        }
                    });

            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new Info { Title = "Unit Viewer API", Version = "v1" });
                    options.DescribeAllEnumsAsStrings();
                    options.OperationFilter<Swagger.CConsumesFilter>();
                    options.OperationFilter<Swagger.CProducesFilter>();
                });

            services
                .AddModelAccessAHU()
                .AddForgeAPI();
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            app.UseCors("AllowAll");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseAuthentication();
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(
                i =>
                {
                    i.SwaggerEndpoint("v1/swagger.json", "Unit Viewer API V1");
                });
        }
    }
}
