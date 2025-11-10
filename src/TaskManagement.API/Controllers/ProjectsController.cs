using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs.Requests;
using TaskManagement.Application.DTOs.Responses;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(
        IProjectService projectService,
        ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>
    /// Lista todos os projetos de um usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <returns>Lista de projetos</returns>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectResponse>>> GetUserProjects(Guid userId)
    {
        _logger.LogInformation("Getting projects for user {UserId}", userId);
        var projects = await _projectService.GetUserProjectsAsync(userId);
        return Ok(projects);
    }

    /// <summary>
    /// Obtém detalhes de um projeto específico
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>Detalhes do projeto</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse>> GetProject(Guid id)
    {
        _logger.LogInformation("Getting project {ProjectId}", id);
        var project = await _projectService.GetProjectByIdAsync(id);
        return Ok(project);
    }

    /// <summary>
    /// Cria um novo projeto
    /// </summary>
    /// <param name="request">Dados do projeto</param>
    /// <returns>Projeto criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectResponse>> CreateProject([FromBody] CreateProjectRequest request)
    {
        _logger.LogInformation("Creating project for user {UserId}", request.UserId);
        var project = await _projectService.CreateProjectAsync(request);
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }

    /// <summary>
    /// Remove um projeto (somente se não tiver tarefas pendentes)
    /// </summary>
    /// <param name="id">ID do projeto</param>
    /// <returns>No content</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        _logger.LogInformation("Deleting project {ProjectId}", id);
        await _projectService.DeleteProjectAsync(id);
        return NoContent();
    }
}
