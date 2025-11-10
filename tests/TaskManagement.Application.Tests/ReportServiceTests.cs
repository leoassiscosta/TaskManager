
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.Services;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Tests;

public class ReportServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);
    private readonly ReportService _sut;

    public ReportServiceTests()
    {
        _uow.SetupGet(u => u.Users).Returns(new Mock<IUserRepository>(MockBehavior.Strict).Object);
        _uow.SetupGet(u => u.Tasks).Returns(new Mock<ITaskRepository>(MockBehavior.Strict).Object);
        _sut = new ReportService(_uow.Object);
    }

    [Fact]
    public async Task GetPerformanceReport_ShouldThrow_WhenRequestingUserNotFound()
    {
        Mock.Get(_uow.Object.Users).Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetPerformanceReportAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetPerformanceReport_ShouldThrow_WhenUserNotManager()
    {
        var user = new User { Id = Guid.NewGuid(), Role = UserRole.User };
        Mock.Get(_uow.Object.Users).Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.GetPerformanceReportAsync(user.Id));
    }

    [Fact]
    public async Task GetPerformanceReport_ShouldReturnAverages_WhenValid()
    {
        var manager = new User { Id = Guid.NewGuid(), Role = UserRole.Manager };
        var other = new User { Id = Guid.NewGuid(), Name = "Ana", Role = UserRole.User };

        Mock.Get(_uow.Object.Users).Setup(r => r.GetByIdAsync(manager.Id)).ReturnsAsync(manager);
        Mock.Get(_uow.Object.Users).Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User> { manager, other });

        Mock.Get(_uow.Object.Tasks).Setup(r => r.GetCompletedTasksCountByUserAsync(manager.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(3);
        Mock.Get(_uow.Object.Tasks).Setup(r => r.GetCompletedTasksCountByUserAsync(other.Id, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(5);

        var report = await _sut.GetPerformanceReportAsync(manager.Id);

        Assert.Equal(2, report.UserPerformances.Count);
        Assert.Equal(4.0, report.AverageTasksCompletedPerUser);
        Assert.All(report.UserPerformances, up => Assert.True(up.TasksCompleted >= 0));
    }
}
