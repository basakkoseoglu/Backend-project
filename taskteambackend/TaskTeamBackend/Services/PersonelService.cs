using System.Data;
using Dapper;

namespace TaskTeamBackend.Services;

public class PersonelService
{
    private readonly IDbConnection _dbConnection;
    private readonly AuthService _authService;

    
    public PersonelService(IDbConnection dbConnection,AuthService authService)
    {
        _dbConnection = dbConnection;
        _authService = authService;
    }
    public async Task<Guid> AddPersonelAsync(string firstName, string lastName, string email, string role, decimal salary, string password)
    {
        var passwordHash = _authService.HashPassword(password);
        
        var query = @"
            INSERT INTO app.personel(first_name, last_name, email, role, salary, password_hash)
            VALUES (@FirstName, @LastName, @Email, @Role, @Salary, @PasswordHash)
            RETURNING id;";
        
        var newId = await _dbConnection.ExecuteScalarAsync<Guid>(query, new
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = role,
            Salary = salary,
            PasswordHash = passwordHash
        });
        
        return newId;
    }
    public async Task<IEnumerable<PersonelDto>> GetPersonelListAsync()
    {
        var query = "SELECT * FROM app.get_personel_list();";
        var personelList = await _dbConnection.QueryAsync<PersonelDto>(query);
        return personelList;
    }

    public async Task<Guid> UpdatePersonelAsync(Guid id, string? firstname, string? lastname, string? role,
        decimal? salary)
    {
      var query="SELECT app.update_personel(@Id,@FirstName,@LastName,@Role,@Salary);";
      var parameters = new
      {
          Id = id,
          FirstName = firstname,
          LastName = lastname,
          Role = role,
          Salary = salary
      };
      var updatedId=await _dbConnection.ExecuteScalarAsync<Guid>(query, parameters);
      return updatedId;
    }

    public async Task<Guid> DeletePersonelAsync(Guid id)
    {
        var query="SELECT app.delete_personel(@Id);";
        var parameters=new {Id=id};
        var deletedId=await _dbConnection.ExecuteScalarAsync<Guid>(query, parameters);
        return deletedId;
    }
}
public class PersonelDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!; 
    public string? Role { get; set; }
    public decimal? Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}