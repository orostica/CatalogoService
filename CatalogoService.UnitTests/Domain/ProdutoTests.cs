using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Enums;
using Xunit;

namespace CatalogoService.UnitTests.Domain;

public class ProdutoTests
{
    private static readonly Guid CategoriaIdPadrao = Guid.NewGuid();

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
