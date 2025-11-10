using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        ITaskService taskService,
        ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todas as tarefas de um projeto
    /// </summary>
    /// <param name="projectId">ID do projeto</param>
    /// <returns>Lista de tarefas</returns>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetProjectTasks(Guid projectId)
    {
        _logger.LogInformation("Getting tasks for project {ProjectId}", projectId);
        var tasks = await _taskService.GetProjectTasksAsync(projectId);
        return Ok(tasks);
    }

    /// <summary>
    /// Obtém detalhes de uma tarefa específica
    /// </summary>
    /// <param name="id">ID da tarefa</param>
    /// <returns>Detalhes da tarefa</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> GetTask(Guid id)
    {
        _logger.LogInformation("Getting task {TaskId}", id);
        var task = await _taskService.GetTaskByIdAsync(id);
        return Ok(task);
    }

    /// <summary>
    /// Cria uma nova tarefa
    /// </summary>
    /// <param name="request">Dados da tarefa</param>
    /// <returns>Tarefa criada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskResponse>> CreateTask([FromBody] CreateTaskRequest request)
    {
        _logger.LogInformation("Creating task for project {ProjectId}", request.ProjectId);
        var task = await _taskService.CreateTaskAsync(request);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    /// <summary>
    /// Atualiza uma tarefa (registra histórico automaticamente)
    /// </summary>
    /// <param name="id">ID da tarefa</param>
    /// <param name="request">Dados a atualizar</param>
    /// <returns>Tarefa atualizada</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskResponse>> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request)
    {
        _logger.LogInformation("Updating task {TaskId}", id);
        var task = await _taskService.UpdateTaskAsync(id, request);
        return Ok(task);
    }

    /// <summary>
    /// Remove uma tarefa
    /// </summary>
    /// <param name="id">ID da tarefa</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        _logger.LogInformation("Deleting task {TaskId}", id);
        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Obtém o histórico de alterações de uma tarefa
    /// </summary>
    /// <param name="id">ID da tarefa</param>
    /// <returns>Lista de alterações</returns>
    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(IEnumerable<TaskHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TaskHistoryResponse>>> GetTaskHistory(Guid id)
    {
        _logger.LogInformation("Getting history for task {TaskId}", id);
        var history = await _taskService.GetTaskHistoryAsync(id);
        return Ok(history);
    }
}
