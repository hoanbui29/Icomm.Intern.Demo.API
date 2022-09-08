using Icomm.API.Intern.Contracts;
using Icomm.API.Intern.DTO.Entities;
using Icomm.API.SmartCity.Sensor.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Icomm.API.Intern.v1;

[Route(RouteConstants.PREFIX + "/v1/student")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly IStudentRepository _repository;

    public StudentController(IStudentRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("add-new-student")]
    public async Task<Student> AddStudent(Student student)
    {
        return await _repository.AddStudent(student);
    }
}