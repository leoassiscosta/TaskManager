namespace TaskManagement.Application.DTOs.Responses;

public class PerformanceReportResponse
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<UserPerformance> UserPerformances { get; set; } = new();
    public double AverageTasksCompletedPerUser { get; set; }
}

public class UserPerformance
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int TasksCompleted { get; set; }
}
