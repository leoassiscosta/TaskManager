
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;
using Xunit;

namespace TaskManagement.Application.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);
    private readonly ProjectService _sut;

    public ProjectServiceTests()
    {
        _uow.SetupGet(u => u.Projects).Returns(new Mock<IProjectRepository>(MockBehavior.Strict).Object);
        _uow.SetupGet(u => u.Users).Returns(new Mock<IUserRepository>(MockBehavior.Strict).Object);
        _sut = new ProjectService(_uow.Object);
    }

    private (Mock<IProjectRepository> projects, Mock<IUserRepository> users) GetRepos()
        => (Mock.Get(_uow.Object.Projects), Mock.Get(_uow.Object.Users));

    [Fact]
    public async Task GetProject_ShouldThrow_WhenNotFound()
    {
        var repos = GetRepos();
        repos.projects.Setup(r => r.GetByIdWithTasksAsync(It.IsAny<Guid>())).ReturnsAsync((Project?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetProjectByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateProject_ShouldThrow_WhenUserNotFound()
    {
        var repos = GetRepos();
        repos.users.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateProjectAsync(new CreateProjectRequest { UserId = Guid.NewGuid(), Name = "X" }));
    }

    [Fact]
    public async Task CreateProject_ShouldCreate_WhenValid()
    {
        // Arrange
        var repos = GetRepos();

        repos.users.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                   .ReturnsAsync(new User { Id = Guid.NewGuid(), Name = "U" });

        var created = new Project { Id = Guid.NewGuid(), Name = "X", UserId = Guid.NewGuid() };

        repos.projects.Setup(r => r.AddAsync(It.IsAny<Project>()))
                      .ReturnsAsync(created);

        // ✅ SaveChangesAsync retorna Task<int>
        _uow.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resp = await _sut.CreateProjectAsync(new CreateProjectRequest
        {
            UserId = created.UserId,
            Name = "X"
        });

        // Assert
        Assert.Equal(created.Id, resp.Id);
        repos.projects.Verify(r => r.AddAsync(It.IsAny<Project>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task DeleteProject_ShouldThrow_WhenNotFound()
    {
        var repos = GetRepos();
        repos.projects.Setup(r => r.GetByIdWithTasksAsync(It.IsAny<Guid>())).ReturnsAsync((Project?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteProjectAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteProject_ShouldThrow_WhenHasPendingTasks()
    {
        var repos = GetRepos();
        var project = new Project { Name = "P", UserId = Guid.NewGuid() };
        // Força HasPendingTasks() retornando true pela coleção Tasks com uma Pending
        var t = new ProjectTask();
        typeof(ProjectTask).GetProperty(nameof(ProjectTask.Status))!.SetValue(t, TaskManagement.Domain.Enums.TasksStatus.Pending);
        typeof(Project).GetProperty(nameof(Project.Tasks))!.SetValue(project, new List<ProjectTask> { t });

        repos.projects.Setup(r => r.GetByIdWithTasksAsync(It.IsAny<Guid>())).ReturnsAsync(project);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.DeleteProjectAsync(Guid.NewGuid()));
    }
}
