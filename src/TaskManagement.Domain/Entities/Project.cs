namespace TaskManagement.Domain.Entities;

public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public const int MaxTasksPerProject = 20;
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    
    public bool CanAddTask()
    {
        return Tasks.Count < MaxTasksPerProject;
    }
    
    public bool HasPendingTasks()
    {
        return Tasks.Any(t => t.Status == Enums.TasksStatus.Pending);
    }
}
