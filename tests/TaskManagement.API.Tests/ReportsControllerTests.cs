
using Xunit;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Tests;

public class ReportsControllerTests
{
    private readonly Mock<IReportService> _service = new(MockBehavior.Strict);
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _controller = new ReportsController(_service.Object, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ReportsController>());
    }

    [Fact]
    public async Task GetPerformanceReport_ShouldOk()
    {
        _service.Setup(s => s.GetPerformanceReportAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new PerformanceReportResponse{ UserPerformances = new(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow });

        var res = await _controller.GetPerformanceReport(Guid.NewGuid());
        Assert.IsType<OkObjectResult>(res.Result);
    }
}
