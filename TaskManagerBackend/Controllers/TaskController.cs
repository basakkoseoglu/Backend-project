using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Collections.Generic;

namespace TaskManagerBackend.Controllers;

[ApiController] 
[Route("api/[controller]")] 
public class TaskController: ControllerBase
{
    private readonly string _connectionString;

    public TaskController(IConfiguration configuration)
    {
        _connectionString=configuration.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var tasks = new List<object>();

        using var conn = new NpgsqlConnection(_connectionString); 
        conn.Open();
        
        using var cmd=new NpgsqlCommand("Select id,title,description,iscompleted,createdat FROM tasks;", conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            tasks.Add(new
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                IsCompleted = reader.GetBoolean(3),
                Created = reader.GetDateTime(4),
            });
        }
        return Ok(tasks);

    }

    [HttpPost]
    public IActionResult CreateTask([FromBody] TaskInput input)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(
            "INSERT INTO tasks (title,description) VALUES (@title,@desc)", conn);
        cmd.Parameters.AddWithValue("title", input.Title);
        cmd.Parameters.AddWithValue("desc", (object?)input.Description ?? DBNull.Value);
        cmd.ExecuteNonQuery();
        return Ok(new {message = "Task created successfully!"});
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTask(int id, [FromBody] TaskInput input)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        
        using var cmd = new NpgsqlCommand("UPDATE tasks SET title=@title,description=@desc, iscompleted=@isCompleted WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("title", input.Title);
        cmd.Parameters.AddWithValue("desc", (object?)input.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("isCompleted", input.IsCompleted ?? false);
        var rowsAffected = cmd.ExecuteNonQuery();
        
        if(rowsAffected == 0)
            return NotFound(new {message = "Task not found!"});
        return Ok(new {message = "Task updated successfully!"});
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        
        using var cmd =new NpgsqlCommand("DELETE FROM tasks WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);
        var rowsAffected = cmd.ExecuteNonQuery();
        if(rowsAffected == 0)
            return  NotFound(new {message = "Task not found!"});
        return Ok(new{message = "Task deleted successfully!"});
    }

    public class TaskInput
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool? IsCompleted { get; set; }
    }
}