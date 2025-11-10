using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.Requests;

public class CreateProjectRequest
{
    [Required(ErrorMessage = "Project name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Project name must be between 1 and 200 characters")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }
}
