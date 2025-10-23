using Microsoft.AspNetCore.Mvc;
using TaskTeamBackend.Services;

namespace TaskTeamBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        
        if (result == null)
            return Unauthorized(new { message = "Email veya şifre hatalı" });
        
        return Ok(result);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
    
        if (result == null)
            return Conflict(new { message = "Bu email adresi zaten kayıtlı." });
    
        return Ok(result);
    }

}