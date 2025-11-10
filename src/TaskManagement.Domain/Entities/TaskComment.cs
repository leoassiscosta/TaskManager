namespace TaskManagement.Domain.Entities;

public class TaskComment : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ProjectTask Task { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
