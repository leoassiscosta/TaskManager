using System.ComponentModel.DataAnnotations;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs.Requests;

public class UpdateTaskRequest
{
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Task title must be between 1 and 200 characters")]
    public string? Title { get; set; }
    
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }
    
    [EnumDataType(typeof(TaskStatus), ErrorMessage = "Invalid status value")]
    public TasksStatus? Status { get; set; }
    
    [Required(ErrorMessage = "User ID is required for tracking changes")]
    public Guid UserId { get; set; } // Usuário que está fazendo a atualização
}
