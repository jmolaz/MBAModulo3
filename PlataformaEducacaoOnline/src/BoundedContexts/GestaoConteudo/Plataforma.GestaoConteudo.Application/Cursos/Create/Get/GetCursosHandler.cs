using MediatR;
using Microsoft.EntityFrameworkCore;
using Plataforma.GestaoConteudo.Infrastructure.Data;

namespace Plataforma.GestaoConteudo.Application.Cursos.Get;

public sealed class GetCursosHandler
    : IRequestHandler<GetCursosQuery, IReadOnlyList<CursoDto>>
{
    private readonly ConteudoDbContext _db;

    public GetCursosHandler(ConteudoDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CursoDto>> Handle(
        GetCursosQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Cursos
            .AsNoTracking()
            .Select(c => new CursoDto(
                c.Id,
                c.Titulo,
                c.Descricao,
                c.Ativo
            ))
            .ToListAsync(cancellationToken);
    }
}
