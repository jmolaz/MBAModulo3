using System;
using Plataforma.GestaoConteudo.Domain.Entities;
using Xunit;

namespace Plataforma.GestaoConteudo.Tests.Domain;

public class CursoTests
{
    [Fact]
    public void CriarCurso_DeveCriarCursoAtivo_ComTituloValido()
    {
        // Arrange
        var titulo = "Curso de Teste";
        var descricao = "Descrição do curso";

        // Act
        var curso = Curso.Criar(titulo, descricao);

        // Assert
        Assert.NotEqual(Guid.Empty, curso.Id);
        Assert.Equal(titulo, curso.Titulo);
        Assert.Equal(descricao, curso.Descricao);
        Assert.True(curso.Ativo);
        Assert.NotEqual(default, curso.CriadoEm);
    }
}
