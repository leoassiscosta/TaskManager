using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;

namespace TaskManagement.Application.Interfaces;

public interface ICommentService
{
    Task<CommentResponse> AddCommentAsync(AddCommentRequest request);
    Task<IEnumerable<CommentResponse>> GetTaskCommentsAsync(Guid taskId);
}
