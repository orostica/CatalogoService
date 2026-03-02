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
    public void Criar_ComDescricao_DeveAtribuirDescricaoCorreta()
    {
        var categoria = Categoria.Create("Eletrônicos", "Produtos eletrônicos em geral");

        Assert.Equal("Produtos eletrônicos em geral", categoria.Descricao);
    }

    [Fact]
    public void Criar_SemDescricao_DeveManterDescricaoNula()
    {
        var categoria = Categoria.Create("Eletrônicos");

        Assert.Null(categoria.Descricao);
    }

    [Fact]
    public void Criar_DeveGerarIdValido()
    {
        var categoria = Categoria.Create("Eletrônicos");

        Assert.NotEqual(Guid.Empty, categoria.Id);
    }

    [Fact]
    public void Criar_DeveInicializarDatasCriacaoEAtualizacao()
    {
        var antes = DateTime.UtcNow;
        var categoria = Categoria.Create("Eletrônicos");
        var depois = DateTime.UtcNow;

        Assert.InRange(categoria.CriadoEm, antes, depois);
        Assert.InRange(categoria.AtualizadoEm, antes, depois);
    }

    [Fact]
    public void Criar_DeveInicializarListaDeProdutosVazia()
    {
        var categoria = Categoria.Create("Eletrônicos");

        Assert.Empty(categoria.Produtos);
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
    public void Atualizar_RemovendoDescricao_DeveDefinirDescricaoComoNula()
    {
        var categoria = Categoria.Create("Eletrônicos", "Descrição antiga");

        categoria.Update("Eletrônicos", null);

        Assert.Null(categoria.Descricao);
    }

    [Fact]
    public void Atualizar_DeveAtualizarDataDeAtualizacao()
    {
        var categoria = Categoria.Create("Eletrônicos");
        var dataAntes = categoria.AtualizadoEm;

        categoria.Update("Informática");

        Assert.True(categoria.AtualizadoEm >= dataAntes);
    }

    [Fact]
    public void Criar_DuasInstancias_DevemTerIdsDistintos()
    {
        var categoria1 = Categoria.Create("Eletrônicos");
        var categoria2 = Categoria.Create("Informática");

        Assert.NotEqual(categoria1.Id, categoria2.Id);
    }
}
