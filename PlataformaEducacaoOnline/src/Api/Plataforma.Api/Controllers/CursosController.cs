using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plataforma.GestaoConteudo.Application.Cursos.Create;
using Plataforma.GestaoConteudo.Application.Cursos.Get;


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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var cursos = await _mediator.Send(new GetCursosQuery(), cancellationToken);
        return Ok(cursos);
    }
}


