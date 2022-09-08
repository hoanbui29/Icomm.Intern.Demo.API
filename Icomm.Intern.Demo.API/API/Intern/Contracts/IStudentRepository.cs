using Icomm.API.Intern.DTO.Entities;

namespace Icomm.API.Intern.Contracts;

public interface IStudentRepository
{
    Task<Student> AddStudent(Student request, CancellationToken cancellationToken = default);
}