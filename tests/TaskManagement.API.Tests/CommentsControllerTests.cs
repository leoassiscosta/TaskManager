
using Xunit;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Tests;

public class CommentsControllerTests
{
    private readonly Mock<ICommentService> _service = new(MockBehavior.Strict);
    private readonly CommentsController _controller;

    public CommentsControllerTests()
    {
        _controller = new CommentsController(_service.Object, new Microsoft.Extensions.Logging.Abstractions.NullLogger<CommentsController>());
    }

    [Fact]
    public async Task AddComment_ShouldReturnCreated()
    {
        var cr = new CommentResponse { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "c" };
        _service.Setup(s => s.AddCommentAsync(It.IsAny<AddCommentRequest>())).ReturnsAsync(cr);

        var res = await _controller.AddComment(new AddCommentRequest { TaskId = cr.TaskId, UserId = cr.UserId, Content = "c" });
        var created = Assert.IsType<CreatedAtActionResult>(res.Result);
        Assert.Equal(cr, created.Value);
    }
}
