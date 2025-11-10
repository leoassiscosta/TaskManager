
using Moq;
using Xunit;
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

namespace TaskManagement.Application.Tests;

public class TaskServiceTests
{
    private readonly Mock<IUnitOfWork> _uow;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _uow = new Mock<IUnitOfWork>(MockBehavior.Strict);

        // Setup common repositories
        _uow.SetupGet(u => u.Tasks).Returns(new Mock<ITaskRepository>(MockBehavior.Strict).Object);
        _uow.SetupGet(u => u.Projects).Returns(new Mock<IProjectRepository>(MockBehavior.Strict).Object);
        _uow.SetupGet(u => u.Users).Returns(new Mock<IUserRepository>(MockBehavior.Strict).Object);
        _uow.SetupGet(u => u.TaskHistory).Returns(new Mock<ITaskHistoryRepository>(MockBehavior.Strict).Object);

        _sut = new TaskService(_uow.Object);
    }

    private (Mock<ITaskRepository> tasks, Mock<IProjectRepository> projects, Mock<IUserRepository> users, Mock<ITaskHistoryRepository> history) GetRepos()
    {
        return (
            Mock.Get(_uow.Object.Tasks),
            Mock.Get(_uow.Object.Projects),
            Mock.Get(_uow.Object.Users),
            Mock.Get(_uow.Object.TaskHistory)
        );
    }

    [Fact]
    public async Task GetTaskById_ShouldThrow_WhenNotFound()
    {
        var repos = GetRepos();
        repos.tasks.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProjectTask?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetTaskByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task CreateTask_ShouldThrow_WhenProjectNotFound()
    {
        var repos = GetRepos();
        var req = new CreateTaskRequest { ProjectId = Guid.NewGuid(), Title = "A" };
        repos.projects.Setup(r => r.GetByIdWithTasksAsync(It.IsAny<Guid>())).ReturnsAsync((Project?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.CreateTaskAsync(req));
    }

    [Fact]
    public async Task CreateTask_ShouldThrow_WhenProjectLimitReached()
    {
        var repos = GetRepos();
        var project = new Project { Name = "P", UserId = Guid.NewGuid() };
        // Simulate max tasks reached -> using CanAddTask() == false via Tasks count == Max
        var tasks = Enumerable.Repeat(new ProjectTask { Title = "t" }, Project.MaxTasksPerProject).ToList();
        typeof(Project).GetProperty(nameof(Project.Tasks))!.SetValue(project, tasks);

        var req = new CreateTaskRequest { ProjectId = Guid.NewGuid(), Title = "A" };
        repos.projects.Setup(r => r.GetByIdWithTasksAsync(It.IsAny<Guid>())).ReturnsAsync(project);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _sut.CreateTaskAsync(req));
    }

    [Fact]
    public async Task CreateTask_ShouldCreate_WhenValid()
    {
        // Arrange
        var repos = GetRepos();
        var project = new Project { Name = "P", UserId = Guid.NewGuid() };
        typeof(Project).GetProperty(nameof(Project.Tasks))!.SetValue(project, new List<ProjectTask>());

        repos.projects.Setup(r => r.GetByIdWithTasksAsync(It.IsAny<Guid>()))
                      .ReturnsAsync(project);

        var created = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Title = "A",
            Status = TasksStatus.Pending
        };

        repos.tasks.Setup(r => r.AddAsync(It.IsAny<ProjectTask>()))
                   .ReturnsAsync(created);

        // ✅ SaveChangesAsync retorna Task<int>, então precisamos retornar um int
        _uow.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resp = await _sut.CreateTaskAsync(new CreateTaskRequest
        {
            ProjectId = project.Id,
            Title = "A"
        });

        // Assert
        Assert.Equal(created.Id, resp.Id);
        repos.tasks.Verify(r => r.AddAsync(It.IsAny<ProjectTask>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task UpdateTask_ShouldThrow_WhenTaskNotFound()
    {
        var repos = GetRepos();
        repos.tasks.Setup(r => r.GetByIdWithHistoryAsync(It.IsAny<Guid>())).ReturnsAsync((ProjectTask?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateTaskAsync(Guid.NewGuid(), new UpdateTaskRequest { UserId = Guid.NewGuid() }));
    }

    [Fact]
    public async Task UpdateTask_ShouldThrow_WhenUserNotFound()
    {
        var repos = GetRepos();
        repos.tasks.Setup(r => r.GetByIdWithHistoryAsync(It.IsAny<Guid>())).ReturnsAsync(new ProjectTask());
        repos.users.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.UpdateTaskAsync(Guid.NewGuid(), new UpdateTaskRequest { UserId = Guid.NewGuid() }));
    }

    [Fact]
    public async Task UpdateTask_ShouldRecordHistory_WhenFieldsChange()
    {
        // Arrange
        var repos = GetRepos();
        var task = new ProjectTask
        {
            Title = "Old",
            Description = "Old",
            DueDate = DateTime.UtcNow.Date,
            Status = TasksStatus.Pending
        };

        repos.tasks.Setup(r => r.GetByIdWithHistoryAsync(It.IsAny<Guid>()))
                   .ReturnsAsync(task);

        repos.users.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                   .ReturnsAsync(new User());

        repos.history.Setup(r => r.AddAsync(It.IsAny<TaskHistory>()))
                     .ReturnsAsync(new TaskHistory());

        repos.tasks.Setup(r => r.UpdateAsync(It.IsAny<ProjectTask>()))
                   .Returns(Task.CompletedTask);

        // ✅ SaveChangesAsync retorna Task<int>
        _uow.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        var req = new UpdateTaskRequest
        {
            UserId = Guid.NewGuid(),
            Title = "New",
            Description = "New",
            DueDate = task.DueDate.AddDays(1),
            Status = TasksStatus.InProgress
        };

        // Act
        var resp = await _sut.UpdateTaskAsync(Guid.NewGuid(), req);

        // Assert
        Assert.Equal("New", resp.Title);
        repos.history.Verify(r => r.AddAsync(It.IsAny<TaskHistory>()), Times.AtLeastOnce);
        repos.tasks.Verify(r => r.UpdateAsync(It.IsAny<ProjectTask>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task DeleteTask_ShouldThrow_WhenNotFound()
    {
        var repos = GetRepos();
        repos.tasks.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProjectTask?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.DeleteTaskAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteTask_ShouldDelete_WhenFound()
    {
        // Arrange
        var repos = GetRepos();
        var task = new ProjectTask { Id = Guid.NewGuid() };

        repos.tasks.Setup(r => r.GetByIdAsync(task.Id))
                   .ReturnsAsync(task);

        repos.tasks.Setup(r => r.DeleteAsync(task))
                   .Returns(Task.CompletedTask);

        _uow.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _sut.DeleteTaskAsync(task.Id);

        // Assert
        repos.tasks.Verify(r => r.GetByIdAsync(task.Id), Times.Once);
        repos.tasks.Verify(r => r.DeleteAsync(task), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task GetTaskHistory_ShouldThrow_WhenTaskNotFound()
    {
        var repos = GetRepos();
        repos.tasks.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProjectTask?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetTaskHistoryAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetTaskHistory_ShouldReturnItems_WhenFound()
    {
        var repos = GetRepos();
        var taskId = Guid.NewGuid();
        repos.tasks.Setup(r => r.GetByIdAsync(taskId)).ReturnsAsync(new ProjectTask { Id = taskId });
        repos.history.Setup(r => r.GetByTaskIdAsync(taskId)).ReturnsAsync(new List<TaskHistory>
        {
            new TaskHistory { TaskId = taskId, ChangeDescription = "c1" },
            new TaskHistory { TaskId = taskId, ChangeDescription = "c2" }
        });

        var items = await _sut.GetTaskHistoryAsync(taskId);

        Assert.Equal(2, items.Count());
    }
}
