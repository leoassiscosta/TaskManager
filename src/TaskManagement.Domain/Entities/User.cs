using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    
    // Navigation properties
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
}
