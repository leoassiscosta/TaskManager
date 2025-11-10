using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CommentResponse> AddCommentAsync(AddCommentRequest request)
    {
        // Validar se a tarefa existe
        var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId);
        if (task == null)
            throw new NotFoundException(nameof(ProjectTask), request.TaskId);

        // Validar se o usuário existe
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        // Criar comentário
        var comment = new TaskComment
        {
            TaskId = request.TaskId,
            UserId = request.UserId,
            Content = request.Content
        };

        var createdComment = await _unitOfWork.TaskComments.AddAsync(comment);

        // REGRA DE NEGÓCIO 6: Registrar comentários no histórico
        var historyEntry = new TaskHistory
        {
            TaskId = request.TaskId,
            UserId = request.UserId,
            ChangeDescription = "Comment added",
            NewValue = request.Content
        };

        await _unitOfWork.TaskHistory.AddAsync(historyEntry);
        await _unitOfWork.SaveChangesAsync();

        return new CommentResponse
        {
            Id = createdComment.Id,
            TaskId = createdComment.TaskId,
            UserId = createdComment.UserId,
            Content = createdComment.Content,
            CreatedAt = createdComment.CreatedAt
        };
    }

    public async Task<IEnumerable<CommentResponse>> GetTaskCommentsAsync(Guid taskId)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        if (task == null)
            throw new NotFoundException(nameof(ProjectTask), taskId);

        var comments = await _unitOfWork.TaskComments.GetByTaskIdAsync(taskId);
        
        return comments.Select(c => new CommentResponse
        {
            Id = c.Id,
            TaskId = c.TaskId,
            UserId = c.UserId,
            Content = c.Content,
            CreatedAt = c.CreatedAt
        });
    }
}
