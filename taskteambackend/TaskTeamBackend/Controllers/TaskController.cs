using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTeamBackend.Services;

namespace TaskTeamBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController: ControllerBase
{
    private readonly TaskService _taskService;
    
    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddTask([FromBody] TaskCreateDto dto)
    {
        var newId = await _taskService.AddTaskAsync(
            dto.ProjectId,
            dto.AssignedPersonnelId,
            dto.TaskTitle,
            dto.TaskDescription,
            dto.Priority
        );
        return Ok(new { Id = newId });
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }
    
    [HttpGet("project/{projectId}")]
    [Authorize]
    public async Task<IActionResult> GetTasksByProject(Guid projectId)
    {
        var tasks = await _taskService.GetTasksByProjectAsync(projectId);
        return Ok(tasks);
    }
    
    [HttpGet("personnel/{personnelId}")]
    [Authorize]
    public async Task<IActionResult> GetTasksByPersonnel(Guid personnelId)
    {
        var tasks = await _taskService.GetTasksByPersonnelAsync(personnelId);
        return Ok(tasks);
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskUpdateDto dto)
    {
        var updatedId = await _taskService.UpdateTaskAsync(
            id,
            dto.TaskTitle,
            dto.TaskDescription,
            dto.Status,
            dto.Priority
        );
        return Ok(new { Id = updatedId });
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var deletedId = await _taskService.DeleteTaskAsync(id);
        return Ok(new { Id = deletedId });
    }
}