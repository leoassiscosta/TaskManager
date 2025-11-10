using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository : Repository<ProjectTask>, ITaskRepository
{
    public TaskRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ProjectTask>> GetByProjectIdAsync(Guid projectId)
    {
        return await _dbSet
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<ProjectTask?> GetByIdWithHistoryAsync(Guid id)
    {
        return await _dbSet
            .Include(t => t.History)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<int> GetCompletedTasksCountByUserAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(t => t.Project)
            .Where(t => t.Project.UserId == userId 
                     && t.Status == TasksStatus.Completed
                     && t.UpdatedAt >= startDate 
                     && t.UpdatedAt <= endDate)
            .CountAsync();
    }
}
