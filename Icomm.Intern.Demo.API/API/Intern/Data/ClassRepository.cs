using Icomm.API.Intern.Contracts;
using Icomm.API.Intern.DTO.Entities;
using Nest;

namespace Icomm.API.Intern.Data;

public class ClassRepository : IClassRepository
{
    private readonly IElasticClient _client;

    public ClassRepository(IElasticClient client)
    {
        _client = client;
    }

    public async Task<Class> CreateClass(Class request, CancellationToken cancellationToken = default)
    {
        var response = await _client.IndexDocumentAsync(request, cancellationToken);
        if (!response.IsValid)
            throw new InvalidOperationException("Couldn't create class");
        return request;
    }
}