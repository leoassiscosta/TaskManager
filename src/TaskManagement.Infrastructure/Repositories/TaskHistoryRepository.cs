using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskHistoryRepository : Repository<TaskHistory>, ITaskHistoryRepository
{
    public TaskHistoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(Guid taskId)
    {
        return await _dbSet
            .Where(h => h.TaskId == taskId)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync();
    }
}
