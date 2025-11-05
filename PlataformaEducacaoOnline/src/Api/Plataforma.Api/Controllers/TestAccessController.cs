using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Plataforma.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestAccessController : ControllerBase
{
    [HttpGet("publico")]
    [AllowAnonymous]
    public IActionResult GetPublico()
        => Ok("✅ Endpoint público — acesso liberado para qualquer um.");

    [HttpGet("aluno")]
    [Authorize(Policy = "AlunoOnly")]
    public IActionResult GetAlunoArea()
        => Ok("🎓 Acesso liberado apenas para ALUNO.");

    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAdminArea()
        => Ok("🛠️ Acesso liberado apenas para ADMIN.");
}
