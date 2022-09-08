using Icomm.API.Intern.Contracts;
using Icomm.API.Intern.Data;
using Icomm.API.Intern.DTO.Entities;
using Icomm.Hosting.Contracts;

namespace Icomm.API.SmartCity.Sensor.Infrastructure.Installers
{
    internal class RegisterContractMappings : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddConfigManager(configuration);
            services.AddSingleton<IStudentRepository, StudentRepository>();
            services.AddSingleton<IClassRepository, ClassRepository>();
        }
    }
}