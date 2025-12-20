using MediatR;

namespace Plataforma.GestaoConteudo.Application.Cursos.Get;

public sealed record GetCursosQuery()
    : IRequest<IReadOnlyList<CursoDto>>;
