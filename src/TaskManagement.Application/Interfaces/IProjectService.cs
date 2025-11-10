using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;

namespace TaskManagement.Application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponse>> GetUserProjectsAsync(Guid userId);
    Task<ProjectResponse> GetProjectByIdAsync(Guid projectId);
    Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request);
    Task DeleteProjectAsync(Guid projectId);
}
