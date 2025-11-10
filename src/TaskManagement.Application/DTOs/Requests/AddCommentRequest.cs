using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Application.DTOs.Requests;

public class AddCommentRequest
{
    [Required(ErrorMessage = "Task ID is required")]
    public Guid TaskId { get; set; }
    
    [Required(ErrorMessage = "User ID is required")]
    public Guid UserId { get; set; }
    
    [Required(ErrorMessage = "Comment content is required")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 5000 characters")]
    public string Content { get; set; } = string.Empty;
}
