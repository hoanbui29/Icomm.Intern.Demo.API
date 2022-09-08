using Icomm.API.Intern.Contracts;
using Icomm.API.Intern.DTO.Entities;
using Nest;

namespace Icomm.API.Intern.Data;

public class StudentRepository : IStudentRepository
{
    private readonly IElasticClient _client;

    public StudentRepository(IElasticClient client)
    {
        _client = client;
    }

    public async Task<Student> AddStudent(Student request, CancellationToken cancellationToken = default)
    {
        var response = await _client.IndexDocumentAsync(request, cancellationToken);
        if (!response.IsValid)
            throw new InvalidOperationException("Couldn't create student");
        return request;
    }
    
    
    
    
}