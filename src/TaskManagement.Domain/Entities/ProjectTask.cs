using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class ProjectTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public Enums.TasksStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public Guid ProjectId { get; set; }
    
    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual ICollection<TaskHistory> History { get; set; } = new List<TaskHistory>();
    public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
}
