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
    public async Task<IActionResult> GetPersonelList()
    {
        var personelList = await _personelService.GetPersonelListAsync();
        return Ok(personelList);
    }
    
    //post api/personel
    [HttpPost]
    public async Task<IActionResult> AddPersonel([FromBody] PersonelCreateDto dto)
    {
        var newId = await _personelService.AddPersonelAsync(dto.FirstName, dto.LastName, dto.Role, dto.Salary);
        return Ok(new{Id=newId});
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePersonel(Guid id, [FromBody] PersonelCreateDto dto)
    {
        var updatedId=await _personelService.UpdatePersonelAsync(id,dto.FirstName,dto.LastName,dto.Role,dto.Salary);
        return Ok(new{UpdatedPersonelId=updatedId});
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePersonel(Guid id)
    {
        var deletedId=await _personelService.DeletePersonelAsync(id);
        return Ok(new{DeletedPersonelId=deletedId});
    }
}

// dto:frontendden gelen vrileri taşır

public class PersonelCreateDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;
    public decimal Salary { get; set; }
}