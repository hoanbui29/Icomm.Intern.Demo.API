using Autofac;
using AutoWrapper;
using EasyCaching.Interceptor.Castle;
using Icomm.AspNetCore.Extensions;
using Icomm.Infrastructure.Configs;
using Icomm.Infrastructure.Extensions;
using Icomm.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Serialization;

namespace Icomm
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
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.MultipartHeadersLengthLimit = int.MaxValue;
            });
            //Register services in Installers folder
            services.AddServicesInAssembly(Configuration, typeof(Startup));

            //Register MVC/Web API, NewtonsoftJson and add FluentValidation Support
            services
                .AddControllers(opt => { opt.Filters.Add(typeof(ValidatorActionFilter)); })
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    opt.SerializerSettings.ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new Newtonsoft.Json.Serialization.SnakeCaseNamingStrategy()
                    };
                    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .AddJsonOptions(opt =>
                {
                    opt.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                });
            //.AddFluentValidation(fv =>
            //{
            //	fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            //	fv.ImplicitlyValidateChildProperties = true;
            //	fv.RegisterValidatorsFromAssemblies(new[] { Assembly.GetAssembly(typeof(Startup)), Assembly.GetAssembly(typeof(ListQueryValidator)), Assembly.GetAssembly(typeof(TimeRangeRequestValidator)) });
            //	//fv.RegisterValidatorsFromAssemblyContaining<Startup>();
            //	//fv.RegisterValidatorsFromAssemblyContaining<ListQuery>();
            //});

            //Register Automapper
            services.AddAutoMapper(typeof(MappingProfileConfiguration));
            
            services.AddElasticsearch(Configuration);
        }

        // for castle
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.ConfigureCastleInterceptor();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            //Enable Swagger and SwaggerUI
            app.UseSwagger(opt => { opt.RouteTemplate = "/swagger/{documentName}/swagger.json"; })
                //.UseReDoc(c =>
                //{
                //	c.RoutePrefix = "swagger/redoc";
                //	c.SpecUrl("/swagger/v1/swagger.json");
                //})
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    c.EnableDeepLinking();
                    c.EnableFilter();
                    //c.MaxDisplayedTags(5);
                    c.ShowExtensions();
                    c.ShowCommonExtensions();
                    c.DisplayRequestDuration();
                    //c.InjectStylesheet("https://cdn.jsdelivr.net/npm/swagger-ui-themes@3.0.0/themes/3.x/theme-material.css");
                    c.InjectStylesheet(
                        "https://cdn.jsdelivr.net/npm/swagger-ui-themes@3.0.0/themes/3.x/theme-flattop.css");

                    c.OAuthClientId("AdminClientId");
                    c.OAuthClientSecret("AdminClientSecret");
                    c.OAuthAppName("Smartcity Intern API - Swagger");
                    c.OAuthUsePkce();
                    //c.OAuthScopeSeparator(" ");
                });
            //.UseHealthChecksUI(setup =>
            //{
            //    setup.AddCustomStylesheet($"{env.ContentRootPath}/Infrastructure/HealthChecks/Ux/branding.css");
            //});

            //Enable AutoWrapper.Core
            //More info see: https://github.com/proudmonkey/AutoWrapper
            app.UseApiResponseAndExceptionWrapper(new AutoWrapperOptions
            {
#if DEBUG
                IsDebug = true,
#endif
                UseApiProblemDetailsException = true,
                IsApiOnly = false,
                WrapWhenApiPathStartsWith = "/api",
                BypassHTMLValidation = true,
                ShowStatusCode = true,
            });

            //Enable AspNetCoreRateLimit
            //if (!env.IsProduction())
            //	app.UseIpRateLimiting();

            app.UseRouting();

            //Enable CORS
            app.UseCors("AllowAll");

            //if (!env.IsDevelopment())
            //	app.UseHttpReports();
            //Adds authenticaton middleware to the pipeline so authentication will be performed automatically on each request to host
            app.UseAuthentication();

            //Adds authorization middleware to the pipeline to make sure the Api endpoint cannot be accessed by anonymous clients
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
            //app.UseSpa(builder =>
            //{
            //	builder.UseProxyToSpaDevelopmentServer("http://10.9.2.151:31255/");
            //});
        }
    }
}