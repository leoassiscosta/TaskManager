using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Interfaces;

public interface ITaskCommentRepository : IRepository<TaskComment>
{
    Task<IEnumerable<TaskComment>> GetByTaskIdAsync(Guid taskId);
}
