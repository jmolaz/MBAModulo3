using MediatR;
using Microsoft.EntityFrameworkCore;
using Plataforma.GestaoConteudo.Infrastructure.Data;

namespace Plataforma.GestaoConteudo.Application.Cursos.Get;

public sealed class GetCursoByIdHandler
    : IRequestHandler<GetCursoByIdQuery, CursoDto?>
{
    private readonly ConteudoDbContext _db;

    public GetCursoByIdHandler(ConteudoDbContext db)
    {
        _db = db;
    }

    public async Task<CursoDto?> Handle(
        GetCursoByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Cursos
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CursoDto(
                c.Id,
                c.Titulo,
                c.Descricao,
                c.Ativo
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
