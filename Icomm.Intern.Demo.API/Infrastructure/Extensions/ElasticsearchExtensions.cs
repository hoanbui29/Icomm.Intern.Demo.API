using Icomm.API.Intern.DTO.Entities;
using Nest;

namespace Icomm.Infrastructure.Extensions;

public static class ElasticsearchExtensions
{
    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["elasticsearch:url"];
        var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex("student")
                .DefaultMappingFor<Class>(m => m
                    .IndexName("class")
                    .IdProperty(prop => prop.Id)
                )
            ;
        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);
    }
}