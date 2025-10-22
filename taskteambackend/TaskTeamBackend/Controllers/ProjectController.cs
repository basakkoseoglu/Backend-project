using Microsoft.AspNetCore.Mvc;
using TaskTeamBackend.Services;

namespace TaskTeamBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController:ControllerBase
{
    private readonly ProjectService _projectService;

    public ProjectController(ProjectService projectService)
    {
        _projectService = projectService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddProject(ProjectCreateDto dto)
    {
        var id = await _projectService.AddProjectAsync(dto.ProjectName,dto.ProjectDescription, dto.OwnerId,dto.StartDate,dto.EndDate);
        return Ok(id);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        var list = await _projectService.GetAllProjectsAsync();
        return Ok(list);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id,[FromBody] ProjectUpdateDto dto)
    {
        var updatedId= await _projectService.UpdateProjectAsync(
            id,
            dto.ProjectName,
            dto.ProjectDescription,
            dto.StartDate,
            dto.EndDate
        );

        return Ok(new { id = updatedId });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var deletedId= await _projectService.DeleteProjectAsync(id);
        return Ok(new{DeletedProjectId=deletedId});
    }
}