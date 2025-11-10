using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskRepository : IRepository<ProjectTask>
{
    Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(Guid projectId);
    Task<ProjectTask?> GetByIdWithHistoryAsync(Guid id);
    Task<int> GetCompletedTasksCountByUserAsync(Guid userId, DateTime startDate, DateTime endDate);
}
