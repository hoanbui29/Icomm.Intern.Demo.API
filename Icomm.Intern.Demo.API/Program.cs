using System;
using Autofac.Extensions.DependencyInjection;
using Icomm.Configs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Icomm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();
            var logger = builder.Services.GetService<ILogger<Program>>();
            try
            {
                logger.LogInformation("Starting web host");
                builder.Run();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Host unexpectedly terminated");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, configBuilder) =>
                    configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{host.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.Logs.json")
                        // .AddConfigManagerHttpProvider(environment: host.HostingEnvironment.EnvironmentName)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args)
                )
                .UseIcommLog()
                //.ConfigureServices((hostContext, services) =>
                //{
                //	services.AddConfigManager(hostContext.Configuration);
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureKestrel(opt =>
                        {
                            opt.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
                            opt.Limits.MaxRequestBodySize = long.MaxValue;
                        })
                        .ConfigureKestrel(opt =>
                        {
                            opt.Limits.MaxRequestBodySize = int.MaxValue;
                        })
#if !DEBUG
						.UseUrls("http://0.0.0.0:5000")
#endif
                        ;
                })
                // for aspcectcore
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}