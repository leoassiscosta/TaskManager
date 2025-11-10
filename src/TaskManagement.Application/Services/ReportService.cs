using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PerformanceReportResponse> GetPerformanceReportAsync(Guid requestingUserId)
    {
        // REGRA DE NEGÓCIO 5: Apenas gerentes podem acessar relatórios
        var requestingUser = await _unitOfWork.Users.GetByIdAsync(requestingUserId);
        
        if (requestingUser == null)
            throw new NotFoundException(nameof(User), requestingUserId);

        if (requestingUser.Role != UserRole.Manager)
        {
            throw new BusinessRuleException(
                "Access denied. Only users with 'Manager' role can access performance reports.");
        }

        // Calcular data de 30 dias atrás
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddDays(-30);

        // Buscar todos os usuários
        var allUsers = await _unitOfWork.Users.GetAllUsersAsync();
        
        var userPerformances = new List<UserPerformance>();

        foreach (var user in allUsers)
        {
            var completedTasksCount = await _unitOfWork.Tasks
                .GetCompletedTasksCountByUserAsync(user.Id, startDate, endDate);

            userPerformances.Add(new UserPerformance
            {
                UserId = user.Id,
                UserName = user.Name,
                TasksCompleted = completedTasksCount
            });
        }

        var averageTasksCompleted = userPerformances.Any() 
            ? userPerformances.Average(up => up.TasksCompleted) 
            : 0;

        return new PerformanceReportResponse
        {
            StartDate = startDate,
            EndDate = endDate,
            UserPerformances = userPerformances,
            AverageTasksCompletedPerUser = Math.Round(averageTasksCompleted, 2)
        };
    }
}
