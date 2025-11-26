using System;

namespace Plataforma.GestaoConteudo.Domain.Entities
{
    public class Curso
    {
        public Guid Id { get; private set; }
        public string Titulo { get; private set; } = null!;
        public string? Descricao { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime? AtualizadoEm { get; private set; }

        // Construtor protegido para o EF
        protected Curso() { }

        private Curso(string titulo, string? descricao)
        {
            Id = Guid.NewGuid();
            DefinirTituloDescricao(titulo, descricao);
            Ativo = true;
            CriadoEm = DateTime.UtcNow;
        }

        public static Curso Criar(string titulo, string? descricao)
        {
            return new Curso(titulo, descricao);
        }

        public void Atualizar(string titulo, string? descricao)
        {
            DefinirTituloDescricao(titulo, descricao);
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

        private void DefinirTituloDescricao(string titulo, string? descricao)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("Título do curso é obrigatório.", nameof(titulo));

            if (titulo.Length > 200)
                throw new ArgumentException("Título do curso não pode passar de 200 caracteres.", nameof(titulo));

            Titulo = titulo.Trim();
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        }
    }
}
