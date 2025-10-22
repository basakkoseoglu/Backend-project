using System.Data;
using Dapper;

namespace TaskTeamBackend.Services;

public class ProjectService
{
    private readonly IDbConnection _dbConnection;
    
    public ProjectService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<Guid> AddProjectAsync(string projectName, string projectDescription, Guid ownerId, DateTime? startDate, DateTime? endDate)
    {
        var query = "SELECT app.add_project(@ProjectName, @ProjectDescription, @OwnerId);";
        var parameters = new
        {
            ProjectName =  projectName, ProjectDescription = projectDescription,OwnerId=ownerId
        };
        var newId = await _dbConnection.ExecuteScalarAsync<Guid>(query,parameters);
        return newId;
    }

    public async Task<IEnumerable<ProjectReadDto>> GetAllProjectsAsync()
    {
        var query = "SELECT * FROM app.get_project_list();";
        var result = await _dbConnection.QueryAsync<ProjectReadDto>(query);
        return result;
    }

    public async Task<Guid> UpdateProjectAsync(Guid id, string projectName, string projectDescription, DateTime? startDate, DateTime? endDate)
    {
        var query = "SELECT app.update_project(@Id::uuid, @ProjectName::varchar, @ProjectDescription::text, @StartDate::date, @EndDate::date);";
        var result=await _dbConnection.ExecuteScalarAsync<Guid>(query, new
        {
            Id = id,
            ProjectName = projectName,
            ProjectDescription = projectDescription,
            StartDate = (object?)startDate ?? DBNull.Value,
            EndDate = (object?)endDate ?? DBNull.Value
        });
        return result;
    }
    public async Task<Guid> DeleteProjectAsync(Guid id)
    {
        var query = "SELECT app.delete_project(@Id);";
        var result=await _dbConnection.ExecuteScalarAsync<Guid>(query, new { Id = id });
        return result;
    }
}
public class ProjectCreateDto
{
    public string ProjectName { get; set; } = null!;
    public string ProjectDescription { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ProjectReadDto
{
    public Guid Id { get; set; }
    public string ProjectName { get; set; } = null!;
    public string ProjectDescription { get; set; } = null!;
    public Guid OwnerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime ProjectCreatedAt { get; set; }
    public DateTime ProjectUpdatedAt { get; set; }
}

public class ProjectUpdateDto
{
    public string ProjectName { get; set; } = null!;
    public string ProjectDescription { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}