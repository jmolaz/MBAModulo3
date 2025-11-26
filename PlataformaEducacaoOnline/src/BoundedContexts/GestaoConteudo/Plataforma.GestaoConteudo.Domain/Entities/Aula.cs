using System;

namespace Plataforma.GestaoConteudo.Domain.Entities
{
    public class Aula
    {
        public Guid Id { get; private set; }
        public Guid CursoId { get; private set; }
        public string Titulo { get; private set; } = null!;
        public string? Descricao { get; private set; }
        public int Ordem { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime? AtualizadoEm { get; private set; }

        // Construtor protegido para EF
        protected Aula() { }

        private Aula(Guid cursoId, string titulo, string? descricao, int ordem)
        {
            Id = Guid.NewGuid();
            CursoId = cursoId;
            DefinirDados(titulo, descricao, ordem);
            Ativo = true;
            CriadoEm = DateTime.UtcNow;
        }

        public static Aula Criar(Guid cursoId, string titulo, string? descricao, int ordem)
        {
            return new Aula(cursoId, titulo, descricao, ordem);
        }

        public void Atualizar(string titulo, string? descricao, int ordem)
        {
            DefinirDados(titulo, descricao, ordem);
            AtualizadoEm = DateTime.UtcNow;
        }

        public void Desativar()
        {
            Ativo = false;
            AtualizadoEm = DateTime.UtcNow;
        }

        public void Reativar()
        {
            Ativo = true;
            AtualizadoEm = DateTime.UtcNow;
        }

        private void DefinirDados(string titulo, string? descricao, int ordem)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("Título da aula é obrigatório.", nameof(titulo));

            if (ordem <= 0)
                throw new ArgumentException("A ordem da aula deve ser maior que zero.", nameof(ordem));

            Titulo = titulo.Trim();
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
            Ordem = ordem;
        }
    }
}
