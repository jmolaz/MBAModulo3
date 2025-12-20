using MediatR;

namespace Plataforma.GestaoConteudo.Application.Cursos.Create;

public sealed record CreateCursoCommand(
    string Nome,
    string? Descricao
) : IRequest<Guid>;
