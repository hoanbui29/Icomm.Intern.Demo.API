namespace Icomm.API.Intern.DTO.Entities;

public class Class
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime created_time { get; } = DateTime.Now;

    public string Name { get; set; }
    
}