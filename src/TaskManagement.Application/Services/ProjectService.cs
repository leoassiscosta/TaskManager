using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProjectResponse>> GetUserProjectsAsync(Guid userId)
    {
        var projects = await _unitOfWork.Projects.GetByUserIdAsync(userId);
        
        return projects.Select(p => new ProjectResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            UserId = p.UserId,
            TaskCount = p.Tasks.Count,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
    }

    public async Task<ProjectResponse> GetProjectByIdAsync(Guid projectId)
    {
        var project = await _unitOfWork.Projects.GetByIdWithTasksAsync(projectId);
        
        if (project == null)
            throw new NotFoundException(nameof(Project), projectId);

        return new ProjectResponse
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            UserId = project.UserId,
            TaskCount = project.Tasks.Count,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt
        };
    }

    public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request)
    {
        // Validar se o usuário existe
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null)
            throw new NotFoundException(nameof(User), request.UserId);

        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId
        };

        var createdProject = await _unitOfWork.Projects.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return new ProjectResponse
        {
            Id = createdProject.Id,
            Name = createdProject.Name,
            Description = createdProject.Description,
            UserId = createdProject.UserId,
            TaskCount = 0,
            CreatedAt = createdProject.CreatedAt,
            UpdatedAt = createdProject.UpdatedAt
        };
    }

    public async Task DeleteProjectAsync(Guid projectId)
    {
        var project = await _unitOfWork.Projects.GetByIdWithTasksAsync(projectId);
        
        if (project == null)
            throw new NotFoundException(nameof(Project), projectId);

        // REGRA DE NEGÓCIO 2: Não pode remover projeto com tarefas pendentes
        if (project.HasPendingTasks())
        {
            throw new BusinessRuleException(
                "Cannot delete project with pending tasks. Please complete or remove all pending tasks first.");
        }

        await _unitOfWork.Projects.DeleteAsync(project);
        await _unitOfWork.SaveChangesAsync();
    }
}
