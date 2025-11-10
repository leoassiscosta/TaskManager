using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;

    public TaskService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TaskResponse>> GetProjectTasksAsync(Guid projectId)
    {
        var tasks = await _unitOfWork.Tasks.GetByProjectIdAsync(projectId);
        
        return tasks.Select(t => MapToTaskResponse(t));
    }

    public async Task<TaskResponse> GetTaskByIdAsync(Guid taskId)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        
        if (task == null)
            throw new NotFoundException(nameof(ProjectTask), taskId);

        return MapToTaskResponse(task);
    }

    public async Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request)
    {
        // Validar se o projeto existe
        var project = await _unitOfWork.Projects.GetByIdWithTasksAsync(request.ProjectId);
        if (project == null)
            throw new NotFoundException(nameof(Project), request.ProjectId);

        // REGRA DE NEGÓCIO 4: Limite de 20 tarefas por projeto
        if (!project.CanAddTask())
        {
            throw new BusinessRuleException(
                $"Cannot add more tasks. Project has reached the maximum limit of {Project.MaxTasksPerProject} tasks.");
        }

        var task = new ProjectTask
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            Priority = request.Priority,
            ProjectId = request.ProjectId,
            Status = TasksStatus.Pending // Sempre começa como Pending
        };

        var createdTask = await _unitOfWork.Tasks.AddAsync(task);
        await _unitOfWork.SaveChangesAsync();

        return MapToTaskResponse(createdTask);
    }

    public async Task<TaskResponse> UpdateTaskAsync(Guid taskId, UpdateTaskRequest request)
    {
        var task = await _unitOfWork.Tasks.GetByIdWithHistoryAsync(taskId);
        
        if (task == null)
            throw new NotFoundException(nameof(ProjectTask), taskId);

        // Validar se o usuário existe
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        // REGRA DE NEGÓCIO 3: Registrar histórico de alterações
        var changes = new List<TaskHistory>();

        if (request.Title != null && request.Title != task.Title)
        {
            changes.Add(new TaskHistory
            {
                TaskId = taskId,
                UserId = request.UserId,
                ChangeDescription = "Title updated",
                PreviousValue = task.Title,
                NewValue = request.Title
            });
            task.Title = request.Title;
        }

        if (request.Description != null && request.Description != task.Description)
        {
            changes.Add(new TaskHistory
            {
                TaskId = taskId,
                UserId = request.UserId,
                ChangeDescription = "Description updated",
                PreviousValue = task.Description,
                NewValue = request.Description
            });
            task.Description = request.Description;
        }

        if (request.DueDate.HasValue && request.DueDate.Value != task.DueDate)
        {
            changes.Add(new TaskHistory
            {
                TaskId = taskId,
                UserId = request.UserId,
                ChangeDescription = "Due date updated",
                PreviousValue = task.DueDate.ToString("yyyy-MM-dd"),
                NewValue = request.DueDate.Value.ToString("yyyy-MM-dd")
            });
            task.DueDate = request.DueDate.Value;
        }

        if (request.Status.HasValue && request.Status.Value != task.Status)
        {
            changes.Add(new TaskHistory
            {
                TaskId = taskId,
                UserId = request.UserId,
                ChangeDescription = "Status updated",
                PreviousValue = task.Status.ToString(),
                NewValue = request.Status.Value.ToString()
            });
            task.Status = request.Status.Value;
        }

        // Salvar histórico
        foreach (var change in changes)
        {
            await _unitOfWork.TaskHistory.AddAsync(change);
        }

        task.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Tasks.UpdateAsync(task);
        await _unitOfWork.SaveChangesAsync();

        return MapToTaskResponse(task);
    }

    public async Task DeleteTaskAsync(Guid taskId)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        
        if (task == null)
            throw new NotFoundException(nameof(ProjectTask), taskId);

        await _unitOfWork.Tasks.DeleteAsync(task);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<TaskHistoryResponse>> GetTaskHistoryAsync(Guid taskId)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(taskId);
        
        if (task == null)
            throw new NotFoundException(nameof(ProjectTask), taskId);

        var history = await _unitOfWork.TaskHistory.GetByTaskIdAsync(taskId);
        
        return history.Select(h => new TaskHistoryResponse
        {
            Id = h.Id,
            TaskId = h.TaskId,
            UserId = h.UserId,
            ChangeDescription = h.ChangeDescription,
            PreviousValue = h.PreviousValue,
            NewValue = h.NewValue,
            ChangedAt = h.ChangedAt
        });
    }

    private static TaskResponse MapToTaskResponse(ProjectTask task)
    {
        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            Priority = task.Priority,
            ProjectId = task.ProjectId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
