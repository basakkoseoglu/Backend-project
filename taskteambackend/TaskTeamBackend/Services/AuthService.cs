using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskTeamBackend.Models;

namespace TaskTeamBackend.Services;

public class AuthService
{
    private readonly IDbConnection _dbConnection;
    private readonly JwtSettings _jwtSettings;
    
    public AuthService(IDbConnection dbConnection, IOptions<JwtSettings> jwtSettings)
    {
        _dbConnection = dbConnection;
        _jwtSettings = jwtSettings.Value;
    }
    
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    
    public string GenerateJwtToken(Guid userId, string email, string firstName, string lastName, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.GivenName, firstName),
                new Claim(ClaimTypes.Surname, lastName),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var query = @"
            SELECT id, email, password_hash, first_name, last_name, role 
            FROM app.personel 
            WHERE email = @Email AND is_active = true";
        
        var user = await _dbConnection.QueryFirstOrDefaultAsync<PersonelAuthDto>(query, new { Email = email });
        
        if (user == null)
            return null;
        
        if (!VerifyPassword(password, user.PasswordHash))
            return null;
        
        var token = GenerateJwtToken(user.Id, user.Email, user.FirstName, user.LastName, user.Role);
        
        return new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role
        };
    }
    public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
    {
        var checkQuery = "SELECT COUNT(*) FROM app.personel WHERE email = @Email";
        var exists = await _dbConnection.ExecuteScalarAsync<int>(checkQuery, new { request.Email });
        if (exists > 0)
            return null; 

        var passwordHash = HashPassword(request.Password);

        var insertQuery = @"
        INSERT INTO app.personel (first_name, last_name, email, role, salary, password_hash, is_active)
        VALUES (@FirstName, @LastName, @Email, @Role, @Salary, @PasswordHash, true)
        RETURNING id;";

        var newId = await _dbConnection.ExecuteScalarAsync<Guid>(insertQuery, new
        {
            request.FirstName,
            request.LastName,
            request.Email,
            request.Role,
            request.Salary,
            PasswordHash = passwordHash
        });

        var token = GenerateJwtToken(newId, request.Email, request.FirstName, request.LastName, request.Role);

        return new RegisterResponse
        {
            UserId = newId,
            Token = token,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role
        };
    }

}
public class PersonelAuthDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class RegisterRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Role { get; set; } = "User";
    public decimal Salary { get; set; }
}

public class RegisterResponse
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;
}
