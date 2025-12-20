using MediatR;

namespace Plataforma.GestaoConteudo.Application.Cursos.Get;

public sealed record GetCursoByIdQuery(Guid Id)
    : IRequest<CursoDto?>;
