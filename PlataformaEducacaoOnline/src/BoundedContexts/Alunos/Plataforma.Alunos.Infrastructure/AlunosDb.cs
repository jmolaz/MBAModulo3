using Microsoft.EntityFrameworkCore;

namespace Plataforma.Alunos.Infrastructure;

public class AlunosDbContext : DbContext
{
    public DbSet<Aluno> Alunos => Set<Aluno>();

    public AlunosDbContext(DbContextOptions<AlunosDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aluno>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.NomeCompleto).HasMaxLength(200);
        });
    }
}

public class Aluno
{
    // Vai compartilhar o mesmo Guid do usuário (Identity) depois
    public Guid Id { get; set; }
    public string? NomeCompleto { get; set; }
}
