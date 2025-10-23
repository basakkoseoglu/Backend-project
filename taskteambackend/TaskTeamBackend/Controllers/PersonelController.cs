using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTeamBackend.Services;

namespace TaskTeamBackend.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PersonelController:ControllerBase
{
    private readonly PersonelService _personelService;
    
    public PersonelController(PersonelService personelService)
    {
        _personelService = personelService;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPersonelList()
    {
        var personelList = await _personelService.GetPersonelListAsync();
        return Ok(personelList);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddPersonel([FromBody] PersonelCreateDto dto)
    {
        var newId = await _personelService.AddPersonelAsync(dto.FirstName, dto.LastName,dto.Email, dto.Role, dto.Salary,dto.Password);
        return Ok(new{Id=newId});
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdatePersonel(Guid id, [FromBody] PersonelUpdateDto dto)
    {
        var updatedId=await _personelService.UpdatePersonelAsync(id,dto.FirstName,dto.LastName,dto.Role,dto.Salary);
        return Ok(new{UpdatedPersonelId=updatedId});
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePersonel(Guid id)
    {
        var deletedId=await _personelService.DeletePersonelAsync(id);
        return Ok(new{DeletedPersonelId=deletedId});
    }
}


public class PersonelCreateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!; 
    public string Role { get; set; } = null!;
    public decimal Salary { get; set; }
    public string Password { get; set; } = null!;
}

public class PersonelUpdateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public decimal? Salary { get; set; }
}