using MediatR;
using Plataforma.GestaoConteudo.Domain.Entities;
using Plataforma.GestaoConteudo.Infrastructure.Data;

namespace Plataforma.GestaoConteudo.Application.Cursos.Create;

public sealed class CreateCursoHandler
    : IRequestHandler<CreateCursoCommand, Guid>
{
    private readonly ConteudoDbContext _db;

    public CreateCursoHandler(ConteudoDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(
        CreateCursoCommand request,
        CancellationToken cancellationToken)
    {
        var curso = Curso.Criar(
            request.Nome,
            request.Descricao
        );

        _db.Cursos.Add(curso);
        await _db.SaveChangesAsync(cancellationToken);

        return curso.Id;
    }
}
