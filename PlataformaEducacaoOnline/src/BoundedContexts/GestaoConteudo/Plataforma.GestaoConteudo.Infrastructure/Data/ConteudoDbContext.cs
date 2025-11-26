using Microsoft.EntityFrameworkCore;
using Plataforma.GestaoConteudo.Domain.Entities;

namespace Plataforma.GestaoConteudo.Infrastructure.Data
{
    public class ConteudoDbContext : DbContext
    {
        public ConteudoDbContext(DbContextOptions<ConteudoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; } = null!;
        public DbSet<Aula> Aulas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Curso
            modelBuilder.Entity<Curso>(entity =>
            {
                entity.ToTable("Cursos");

                entity.HasKey(c => c.Id);

                entity.Property(c => c.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(c => c.CriadoEm)
                    .IsRequired();

                entity.Property(c => c.Ativo)
                    .IsRequired();
            });

            // Aula
            modelBuilder.Entity<Aula>(entity =>
            {
                entity.ToTable("Aulas");

                entity.HasKey(a => a.Id);

                entity.Property(a => a.Titulo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(a => a.Ordem)
                    .IsRequired();

                entity.HasOne<Curso>()
                    .WithMany()
                    .HasForeignKey(a => a.CursoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
