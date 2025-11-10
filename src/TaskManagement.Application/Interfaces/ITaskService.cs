using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetProjectTasksAsync(Guid projectId);
    Task<TaskResponse> GetTaskByIdAsync(Guid taskId);
    Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request);
    Task<TaskResponse> UpdateTaskAsync(Guid taskId, UpdateTaskRequest request);
    Task DeleteTaskAsync(Guid taskId);
    Task<IEnumerable<TaskHistoryResponse>> GetTaskHistoryAsync(Guid taskId);
}
