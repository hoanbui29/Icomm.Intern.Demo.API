using EasyCaching.Core.Interceptor;
using EasyCaching.Interceptor.Castle;
using Icomm.AspNetCore.Contracts;
using Icomm.Hosting.Contracts;
using Icomm.Infrastructure.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Icomm.Infrastructure.Installers
{
	internal class RegisterEasyCaching : IServiceRegistration
	{
		public void RegisterAppServices(IServiceCollection services, IConfiguration config)
		{
			//services.AddScoped<IAspectCoreService, AspectCoreService>();

			services.AddEasyCaching(options =>
			{
				options.UseInMemory(opt =>
				{
					opt.EnableLogging = true;
				}, "m1");
			});
			services.TryAddSingleton<IEasyCachingKeyGenerator, CustomEasyCachingKeyGenerator>();
			// AspectCore
			services.ConfigureCastleInterceptor(options => options.CacheProviderName = "m1");
		}
	}
}