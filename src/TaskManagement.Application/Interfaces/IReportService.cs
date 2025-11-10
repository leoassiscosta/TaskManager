using TaskManagement.Application.DTOs.Responses;

namespace TaskManagement.Application.Interfaces;

public interface IReportService
{
    Task<PerformanceReportResponse> GetPerformanceReportAsync(Guid requestingUserId);
}
