using Icomm.API.Intern.DTO.Entities;

namespace Icomm.API.Intern.Contracts;

public interface IClassRepository
{
    Task<Class> CreateClass(Class request, CancellationToken cancellationToken = default);
}