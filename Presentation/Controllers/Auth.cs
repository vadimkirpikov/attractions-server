using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TouristServer.DAL;
using TouristServer.DAL.Dbo;
using TouristServer.Presentation.Dto;

namespace TouristServer.Presentation.Controllers;

[ApiController]
[Route("v1/auth")]
public class AuthController(ApplicationDbContext context, IOptions<JwtSettings> jwt) : ControllerBase
{
    private readonly PasswordHasher<object> _hasher = new();

    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt.Value.Issuer,
            audience: jwt.Value.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return NotFound();
        }
        if (_hasher.VerifyHashedPassword(user, user.HashPassword, loginDto.Password!) 
            != PasswordVerificationResult.Success)
        {
            return NotFound();
        }

        var refreshToken = user.RefreshToken;
        var token = GenerateToken(user);
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions ()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });
        return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var existingUser = await context.Users.SingleOrDefaultAsync(u => u.Email == registerDto.Email);
        if (existingUser != null)
        {
            return BadRequest("This email is already registered");
        }

        var refreshToken = Guid.NewGuid().ToString();
        var user = new User
        {
            Email = registerDto.Email!,
            Name = registerDto.Name!,
            Id = Guid.NewGuid(),
            Role = "User",
            HashPassword = _hasher.HashPassword(null!, registerDto.Password!),
            RefreshToken = refreshToken
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return Ok(new { Token = GenerateToken(user), RefreshToken = refreshToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAccessToken()
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            return Unauthorized();
        }
        var user = await context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null)
        {
            return Unauthorized();
        }
        
        var newAccessToken = GenerateToken(user);
        
        return Ok(new {Token = newAccessToken});
    }
}