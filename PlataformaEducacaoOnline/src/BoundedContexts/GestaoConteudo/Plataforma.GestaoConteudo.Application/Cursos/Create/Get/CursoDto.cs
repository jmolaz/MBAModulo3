namespace Plataforma.GestaoConteudo.Application.Cursos.Get;

public sealed record CursoDto(
    Guid Id,
    string Titulo,
    string? Descricao,
    bool Ativo
);
