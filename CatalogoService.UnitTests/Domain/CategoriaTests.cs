using CatalogoService.Domain.Entities;
using Xunit;

namespace CatalogoService.UnitTests.Domain;

public class CategoriaTests
{
    [Fact]
    public void Criar_ComNomeValido_DeveAtribuirNomeCorreto()
    {
        var categoria = Categoria.Create("Eletrônicos");

        Assert.Equal("Eletrônicos", categoria.Nome);
    }

    [Fact]
    public void Atualizar_ComNovosValores_DeveSubstituirNomeEDescricao()
    {
        var categoria = Categoria.Create("Eletrônicos", "Descrição antiga");

        categoria.Update("Informática", "Computadores e periféricos");

        Assert.Equal("Informática", categoria.Nome);
        Assert.Equal("Computadores e periféricos", categoria.Descricao);
    }

    [Fact]
    public void Atualizar_DeveAtualizarDataDeAtualizacao()
    {
        var categoria = Categoria.Create("Eletrônicos");
        var dataAntes = categoria.AtualizadoEm;

        categoria.Update("Informática");

        Assert.True(categoria.AtualizadoEm >= dataAntes);
    }
}
