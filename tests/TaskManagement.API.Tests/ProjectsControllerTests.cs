
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.API.Controllers;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Exceptions;
using Xunit;

namespace TaskManagement.API.Tests;

public class ProjectsControllerTests
{
    private readonly Mock<IProjectService> _service = new(MockBehavior.Strict);
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        _controller = new ProjectsController(_service.Object, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ProjectsController>());
    }

    [Fact]
    public async Task GetUserProjects_ShouldOk()
    {
        _service.Setup(s => s.GetUserProjectsAsync(It.IsAny<Guid>())).ReturnsAsync(new List<ProjectResponse>());
        var res = await _controller.GetUserProjects(Guid.NewGuid());
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task CreateProject_ShouldCreated()
    {
        var pr = new ProjectResponse { Id = Guid.NewGuid() };
        _service.Setup(s => s.CreateProjectAsync(It.IsAny<CreateProjectRequest>())).ReturnsAsync(pr);
        var res = await _controller.CreateProject(new CreateProjectRequest { UserId = Guid.NewGuid(), Name = "P" });
        var created = Assert.IsType<CreatedAtActionResult>(res.Result);
        Assert.Equal(pr, created.Value);
    }

    [Fact]
    public void CanAddTask_ShouldReturnFalse_WhenLimitReached()
    {
        var p = new Project();
        typeof(Project).GetProperty(nameof(Project.Tasks))!.SetValue(p,
            Enumerable.Repeat(new ProjectTask(), Project.MaxTasksPerProject).ToList());

        Assert.False(p.CanAddTask());
    }

    [Fact]
    public void HasPendingTasks_ShouldReturnTrue_WhenAnyPending()
    {
        var p = new Project();
        p.Tasks = new List<ProjectTask> { new ProjectTask { Status = TasksStatus.Pending } };
        Assert.True(p.HasPendingTasks());
    }


}
