using Icomm.API.Intern.Contracts;
using Icomm.API.Intern.DTO.Entities;
using Icomm.API.SmartCity.Sensor.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Icomm.API.Intern.v1;

[Route(RouteConstants.PREFIX + "/v1/class")]
[ApiController]
public class ClassController : ControllerBase
{
    private readonly IClassRepository _repository;

    public ClassController(IClassRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("create-class")]
    public async Task<Class> CreateClass(Class request)
    {
        return await _repository.CreateClass(request);
    }
}