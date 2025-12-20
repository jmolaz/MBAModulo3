using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.GestaoConteudo.Application.Cursos.Create;

namespace Plataforma.Api.Controllers;

[ApiController]
[Route("api/cursos")]
[Authorize(Policy = "AdminOnly")]
public sealed class CursosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CursosController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCursoCommand command,
        CancellationToken cancellationToken)
    {
        var cursoId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = cursoId }, cursoId);
    }
}
