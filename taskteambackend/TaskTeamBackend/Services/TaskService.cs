using System.Data;
using Dapper;

namespace TaskTeamBackend.Services;

public class TaskService
{
    private readonly IDbConnection _dbConnection;
    
    public TaskService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    
    public async Task<Guid> AddTaskAsync(Guid projectId, Guid assignedPersonnelId, 
        string taskTitle, string taskDescription, string priority = "Orta")
    {
        var query = @"SELECT app.add_task(
            @ProjectId::uuid, 
            @AssignedPersonnelId::uuid, 
            @TaskTitle::varchar, 
            @TaskDescription::text, 
            @Priority::varchar
        );";
        var newId = await _dbConnection.ExecuteScalarAsync<Guid>(query, new
        {
            ProjectId = projectId,
            AssignedPersonnelId = assignedPersonnelId,
            TaskTitle = taskTitle,
            TaskDescription = taskDescription,
            Priority = priority
        });
        return newId;
    }
    
    public async Task<IEnumerable<TaskReadDto>> GetAllTasksAsync()
    {
        var query = "SELECT * FROM app.get_task_list();";
        var result = await _dbConnection.QueryAsync<TaskReadDto>(query);
        return result;
    }
    
    public async Task<IEnumerable<TaskReadDto>> GetTasksByProjectAsync(Guid projectId)
    {
        var query = "SELECT * FROM app.get_tasks_by_project(@ProjectId);";
        var result = await _dbConnection.QueryAsync<TaskReadDto>(query, new { ProjectId = projectId });
        return result;
    }

    public async Task<IEnumerable<TaskReadDto>> GetTasksByPersonnelAsync(Guid personnelId)
    {
        var query = "SELECT * FROM app.get_tasks_by_personnel(@PersonnelId);";
        var result = await _dbConnection.QueryAsync<TaskReadDto>(query, new { PersonnelId = personnelId });
        return result;
    }
    
    public async Task<Guid> UpdateTaskAsync(Guid id, string taskTitle, 
        string taskDescription, string status, string priority)
    {
        var query = @"SELECT app.update_task(
            @Id::uuid, 
            @TaskTitle::varchar, 
            @TaskDescription::text, 
            @Status::varchar, 
            @Priority::varchar
        );";
        var result = await _dbConnection.ExecuteScalarAsync<Guid>(query, new
        {
            Id = id,
            TaskTitle = taskTitle,
            TaskDescription = taskDescription,
            Status = status,
            Priority = priority
        });
        return result;
    }
    
    public async Task<Guid> DeleteTaskAsync(Guid id)
    {
        var query = "SELECT app.delete_task(@Id);";
        var result = await _dbConnection.ExecuteScalarAsync<Guid>(query, new { Id = id });
        return result;
    }
}

public class TaskCreateDto
{
    public Guid ProjectId { get; set; }
    public Guid AssignedPersonnelId { get; set; }
    public string TaskTitle { get; set; } = null!;
    public string TaskDescription { get; set; } = null!;
    public string Priority { get; set; } = "Orta";
}

public class TaskReadDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid AssignedPersonnelId { get; set; }
    public string TaskTitle { get; set; } = null!;
    public string TaskDescription { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class TaskUpdateDto
{
    public string TaskTitle { get; set; } = null!;
    public string TaskDescription { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Priority { get; set; } = null!;
}