using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Plataforma.Api.Auth;
using Plataforma.GestaoAlunos.Infrastructure;


namespace Plataforma.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _config;

    private readonly AlunosDbContext _alunosDb;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration config,
        AlunosDbContext alunosDb)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
        _alunosDb = alunosDb;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // Garante a role informada
        if (!await _roleManager.RoleExistsAsync(dto.Role))
            await _roleManager.CreateAsync(new IdentityRole<Guid>(dto.Role));

        await _userManager.AddToRoleAsync(user, dto.Role);

        // Se for Aluno, cria a persona com o MESMO Id do usuário
        if (string.Equals(dto.Role, "Aluno", StringComparison.OrdinalIgnoreCase))
        {
            _alunosDb.Alunos.Add(new Aluno
            {
                Id = user.Id,
                NomeCompleto = dto.NomeCompleto
            });
            await _alunosDb.SaveChangesAsync();
        }


        return Ok(new { user.Id, dto.Email, dto.Role });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

       var isAdmin = roles.Contains("Admin");
       var isAluno = roles.Contains("Aluno");
       var persona = isAdmin ? "Admin" : (isAluno ? "Aluno" : "User");


       var claims = new List<Claim>
       {
           new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
           new(JwtRegisteredClaimNames.Email, user.Email!),
           new(ClaimTypes.NameIdentifier, user.Id.ToString()),
           new("persona", persona) 
       };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:ExpiresMinutes")),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { access_token = jwt });
    }
}

public sealed record RegisterDto(string Email, string Password, string Role, string? NomeCompleto);

public sealed record LoginDto(string Email, string Password);
