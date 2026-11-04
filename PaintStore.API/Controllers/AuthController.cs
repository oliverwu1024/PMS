using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaintStore.API.Dtos;
using PaintStore.Models.Methods;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;


namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/auth")]                                    
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;   
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<User> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)  
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }
        return Unauthorized();
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest request)              // 👈 SignupRequest
    {
        var user = new User(request.Username, request.Email, request.Phone, request.Age, 0);  // 👈 your constructor
        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }   
        return BadRequest(result.Errors);
    }



    private string GenerateJwtToken(User user)                          
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   
        };

        // add role claims
        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // give a different scope depending on role
        claims.Add(new Claim("scope", roles.Contains("Admin") ? "admin" : "user"));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));   
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),                   
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
