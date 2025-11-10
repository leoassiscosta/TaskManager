using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskHistoryRepository : IRepository<TaskHistory>
{
    Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(Guid taskId);
}
