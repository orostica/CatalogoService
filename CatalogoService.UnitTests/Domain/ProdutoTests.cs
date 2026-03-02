using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Enums;
using Xunit;

namespace CatalogoService.UnitTests.Domain;

public class ProdutoTests
{
    private static readonly Guid CategoriaIdPadrao = Guid.NewGuid();

    [Fact]
    public void Criar_ComDadosValidos_DeveAtribuirPropriedadesCorretamente()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao, "Notebook gamer", "http://img.com/nb.jpg");

        Assert.Equal("Notebook", produto.Nome);
        Assert.Equal(2999.99m, produto.Preco);
        Assert.Equal(CategoriaIdPadrao, produto.CategoriaId);
        Assert.Equal("Notebook gamer", produto.Descricao);
        Assert.Equal("http://img.com/nb.jpg", produto.ImagemUrl);
    }

    [Fact]
    public void Criar_SemDescricaoESemImagem_DeveManterCamposNulos()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);

        Assert.Null(produto.Descricao);
        Assert.Null(produto.ImagemUrl);
    }

    [Fact]
    public void Criar_DeveGerarIdValido()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);

        Assert.NotEqual(Guid.Empty, produto.Id);
    }

    [Fact]
    public void Criar_DeveDefinirStatusComoDisponivel()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);

        Assert.Equal(ProdutoStatus.Disponivel, produto.Status);
    }

    [Fact]
    public void Reservar_DeveAlterarStatusParaReservado()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);

        produto.Reservar();

        Assert.Equal(ProdutoStatus.Reservado, produto.Status);
    }

    [Fact]
    public void Reservar_DeveAtualizarDataDeAtualizacao()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);
        var dataAntes = produto.AtualizadoEm;

        produto.Reservar();

        Assert.True(produto.AtualizadoEm >= dataAntes);
    }

    [Fact]
    public void Indisponibilizar_DeveAlterarStatusParaIndisponivel()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);
        produto.Reservar();

        produto.Indisponibilizar();

        Assert.Equal(ProdutoStatus.Indisponivel, produto.Status);
    }

    [Fact]
    public void Indisponibilizar_DeveAtualizarDataDeAtualizacao()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);
        var dataAntes = produto.AtualizadoEm;

        produto.Indisponibilizar();

        Assert.True(produto.AtualizadoEm >= dataAntes);
    }

    [Fact]
    public void Disponibilizar_DeveAlterarStatusParaDisponivel()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);
        produto.Indisponibilizar();

        produto.Disponibilizar();

        Assert.Equal(ProdutoStatus.Disponivel, produto.Status);
    }

    [Fact]
    public void Disponibilizar_DeveAtualizarDataDeAtualizacao()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);
        produto.Indisponibilizar();
        var dataAntes = produto.AtualizadoEm;

        produto.Disponibilizar();

        Assert.True(produto.AtualizadoEm >= dataAntes);
    }

    [Fact]
    public void Atualizar_ComNovosValores_DeveSubstituirPropriedades()
    {
        var novaCategoria = Guid.NewGuid();
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao, "Descrição antiga");

        produto.Update("Notebook Pro", 4999.99m, novaCategoria, "Descrição nova", "http://img.com/nbpro.jpg");

        Assert.Equal("Notebook Pro", produto.Nome);
        Assert.Equal(4999.99m, produto.Preco);
        Assert.Equal(novaCategoria, produto.CategoriaId);
        Assert.Equal("Descrição nova", produto.Descricao);
        Assert.Equal("http://img.com/nbpro.jpg", produto.ImagemUrl);
    }

    [Fact]
    public void Atualizar_NaoDeveAlterarStatus()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);
        produto.Reservar();

        produto.Update("Notebook Pro", 4999.99m, CategoriaIdPadrao);

        Assert.Equal(ProdutoStatus.Reservado, produto.Status);
    }

    [Fact]
    public void CicloCompleto_Disponivel_Reservado_Indisponivel_Disponivel_DevePercorrerEstados()
    {
        var produto = Produto.Create("Notebook", 2999.99m, CategoriaIdPadrao);
        Assert.Equal(ProdutoStatus.Disponivel, produto.Status);

        produto.Reservar();
        Assert.Equal(ProdutoStatus.Reservado, produto.Status);

        produto.Indisponibilizar();
        Assert.Equal(ProdutoStatus.Indisponivel, produto.Status);

        produto.Disponibilizar();
        Assert.Equal(ProdutoStatus.Disponivel, produto.Status);
    }
}
