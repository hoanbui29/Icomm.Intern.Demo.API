namespace Icomm.API.Intern.DTO.Entities;

public class Student
{
    public Guid Id { get; } = Guid.NewGuid();

    public DateTime created_time { get; } = DateTime.Now;

    public string Name { get; set; }

    public int Age { get; set; }

    public string Phone { get; set; }

    public string Address { get; set; }

    public float Gpa { get; set; }
}