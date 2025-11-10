
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _service = new(MockBehavior.Strict);
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<TasksController>>().Object;
        _controller = new TasksController(_service.Object, new Microsoft.Extensions.Logging.Abstractions.NullLogger<TasksController>());
    }

    [Fact]
    public async Task GetProjectTasks_ShouldReturnOk()
    {
        _service.Setup(s => s.GetProjectTasksAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<TaskResponse>());

        var result = await _controller.GetProjectTasks(Guid.NewGuid());
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnCreated()
    {
        var tr = new TaskResponse { Id = Guid.NewGuid() };
        _service.Setup(s => s.CreateTaskAsync(It.IsAny<CreateTaskRequest>()))
            .ReturnsAsync(tr);

        var result = await _controller.CreateTask(new CreateTaskRequest { ProjectId = Guid.NewGuid(), Title = "T" });
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(tr, created.Value);
    }
}
