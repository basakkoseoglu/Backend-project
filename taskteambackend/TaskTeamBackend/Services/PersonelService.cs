using System.Data;
using Dapper;

namespace TaskTeamBackend.Services;

public class PersonelService
{
    private readonly IDbConnection _dbConnection;
    public PersonelService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    // personel add
    public async Task<Guid> AddPersonelAsync(string firstName, string lastName, string role, decimal salary)
    {
        var query = "SELECT app.add_personel(@FirstName,@LastName,@Role,@Salary);";
        var parameters = new
        {
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            Salary = salary
        };
        var newId=await _dbConnection.ExecuteScalarAsync<Guid>(query, parameters);
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
    public string? Role { get; set; }
    public decimal? Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}