using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(
        ICommentService commentService,
        ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// Adiciona um comentário a uma tarefa (registrado no histórico)
    /// </summary>
    /// <param name="request">Dados do comentário</param>
    /// <returns>Comentário criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentResponse>> AddComment([FromBody] AddCommentRequest request)
    {
        _logger.LogInformation("Adding comment to task {TaskId}", request.TaskId);
        var comment = await _commentService.AddCommentAsync(request);
        return CreatedAtAction(nameof(GetTaskComments), new { taskId = comment.TaskId }, comment);
    }

    /// <summary>
    /// Lista todos os comentários de uma tarefa
    /// </summary>
    /// <param name="taskId">ID da tarefa</param>
    /// <returns>Lista de comentários</returns>
    [HttpGet("task/{taskId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<CommentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CommentResponse>>> GetTaskComments(Guid taskId)
    {
        _logger.LogInformation("Getting comments for task {TaskId}", taskId);
        var comments = await _commentService.GetTaskCommentsAsync(taskId);
        return Ok(comments);
    }
}
