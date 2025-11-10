using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Requests;

public class CreateTaskRequest
{
    [Required(ErrorMessage = "Task title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Task title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Due date is required")]
    [DataType(DataType.DateTime)]
    public DateTime DueDate { get; set; }
    
    [Required(ErrorMessage = "Priority is required")]
    [EnumDataType(typeof(TaskPriority), ErrorMessage = "Invalid priority value")]
    public TaskPriority Priority { get; set; }
    
    [Required(ErrorMessage = "Project ID is required")]
    public Guid ProjectId { get; set; }
}
