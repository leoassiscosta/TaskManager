
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Tests;

public class CommentServiceTests
{
    private readonly Moq.Mock<IUnitOfWork> _uow = new(MockBehavior.Strict);
    private readonly CommentService _sut;

    public CommentServiceTests()
    {
        _uow.SetupGet(u => u.Tasks).Returns(new Mock<ITaskRepository>(MockBehavior.Strict).Object);
        _uow.SetupGet(u => u.Users).Returns(new Mock<IUserRepository>(MockBehavior.Strict).Object);
        _uow.SetupGet(u => u.TaskComments).Returns(new Mock<ITaskCommentRepository>(MockBehavior.Strict).Object);
        _uow.SetupGet(u => u.TaskHistory).Returns(new Mock<ITaskHistoryRepository>(MockBehavior.Strict).Object);
        _sut = new CommentService(_uow.Object);
    }

    [Fact]
    public async Task AddComment_ShouldThrow_WhenTaskNotFound()
    {
        Mock.Get(_uow.Object.Tasks).Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProjectTask?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.AddCommentAsync(new AddCommentRequest { TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "c" }));
    }

    [Fact]
    public async Task AddComment_ShouldThrow_WhenUserNotFound()
    {
        var tid = Guid.NewGuid();
        Mock.Get(_uow.Object.Tasks).Setup(r => r.GetByIdAsync(tid)).ReturnsAsync(new ProjectTask { Id = tid });
        Mock.Get(_uow.Object.Users).Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.AddCommentAsync(new AddCommentRequest { TaskId = tid, UserId = Guid.NewGuid(), Content = "c" }));
    }

    [Fact]
    public async Task AddComment_ShouldCreate_AndWriteHistory()
    {
        // Arrange
        var tid = Guid.NewGuid();
        var uid = Guid.NewGuid();

        Mock.Get(_uow.Object.Tasks)
            .Setup(r => r.GetByIdAsync(tid))
            .ReturnsAsync(new ProjectTask { Id = tid });

        Mock.Get(_uow.Object.Users)
            .Setup(r => r.GetByIdAsync(uid))
            .ReturnsAsync(new User { Id = uid });

        Mock.Get(_uow.Object.TaskComments)
            .Setup(r => r.AddAsync(It.IsAny<TaskComment>()))
            .ReturnsAsync(new TaskComment { Id = Guid.NewGuid(), TaskId = tid, UserId = uid, Content = "c" });

        Mock.Get(_uow.Object.TaskHistory)
            .Setup(r => r.AddAsync(It.IsAny<TaskHistory>()))
            .ReturnsAsync(new TaskHistory());

        // ✅ SaveChangesAsync retorna Task<int>, então devemos retornar um int
        _uow.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var resp = await _sut.AddCommentAsync(new AddCommentRequest { TaskId = tid, UserId = uid, Content = "c" });

        // Assert
        Assert.Equal(tid, resp.TaskId);
        Assert.Equal(uid, resp.UserId);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task GetTaskComments_ShouldThrow_WhenTaskNotFound()
    {
        Mock.Get(_uow.Object.Tasks).Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProjectTask?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetTaskCommentsAsync(Guid.NewGuid()));
    }
}
