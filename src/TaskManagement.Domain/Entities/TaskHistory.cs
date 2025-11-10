namespace TaskManagement.Domain.Entities;

public class TaskHistory : BaseEntity
{
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string ChangeDescription { get; set; } = string.Empty;
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime ChangedAt { get; set; }
    
    // Navigation properties
    public virtual ProjectTask Task { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    
    public TaskHistory()
    {
        ChangedAt = DateTime.UtcNow;
    }
}
