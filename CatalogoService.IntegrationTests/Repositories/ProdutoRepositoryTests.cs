using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Enums;
using CatalogoService.Infrastructure.Repositories;
using CatalogoService.IntegrationTests.Fixtures;
using Xunit;

namespace CatalogoService.IntegrationTests.Repositories;

public class ProdutoRepositoryTests
{
    private static async Task<Categoria> CriarCategoriaAsync(
        CatalogoService.Infrastructure.Persistence.CatalogoDbContext ctx)
    {
        var repoCategoria = new CategoriaRepository(ctx);
        var categoria = Categoria.Create("Eletrônicos");
        await repoCategoria.AddAsync(categoria);
        return categoria;
    }

    [Fact]
    public async Task Adicionar_DeveInserirProdutoNoContexto()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var categoria = await CriarCategoriaAsync(ctx);
        var repo = new ProdutoRepository(ctx);
        var produto = Produto.Create("Notebook", 2999.99m, categoria.Id, "Desc", "http://img.com");

        await repo.AddAsync(produto);

        var salvo = await repo.GetByIdAsync(produto.Id);
        Assert.NotNull(salvo);
        Assert.Equal("Notebook", salvo.Nome);
    }

    [Fact]
    public async Task ObterPorId_QuandoExiste_DeveRetornarProdutoComCategoria()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var categoria = await CriarCategoriaAsync(ctx);
        var repo = new ProdutoRepository(ctx);
        var produto = Produto.Create("Notebook", 2999.99m, categoria.Id);
        await repo.AddAsync(produto);

        var resultado = await repo.GetByIdAsync(produto.Id);

        Assert.NotNull(resultado);
        Assert.Equal("Notebook", resultado.Nome);
        Assert.NotNull(resultado.Categoria);
        Assert.Equal("Eletrônicos", resultado.Categoria.Nome);
    }

    [Fact]
    public async Task ObterPorId_QuandoNaoExiste_DeveRetornarNull()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new ProdutoRepository(ctx);

        var resultado = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarTodosOsProdutosComCategorias()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var categoria = await CriarCategoriaAsync(ctx);
        var repo = new ProdutoRepository(ctx);
        await repo.AddAsync(Produto.Create("Notebook", 2999m, categoria.Id));
        await repo.AddAsync(Produto.Create("Monitor", 1500m, categoria.Id));

        var resultado = await repo.GetAllAsync();

        var lista = resultado.ToList();
        Assert.Equal(2, lista.Count);
        Assert.All(lista, p => Assert.NotNull(p.Categoria));
    }

    [Fact]
    public async Task ObterPorCategoria_DeveRetornarSomenteOsProdutosDaCategoria()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repoCategoria = new CategoriaRepository(ctx);
        var categoriaA = Categoria.Create("Eletrônicos");
        var categoriaB = Categoria.Create("Vestuário");
        await repoCategoria.AddAsync(categoriaA);
        await repoCategoria.AddAsync(categoriaB);

        var repo = new ProdutoRepository(ctx);
        await repo.AddAsync(Produto.Create("Notebook", 2999m, categoriaA.Id));
        await repo.AddAsync(Produto.Create("Monitor", 1500m, categoriaA.Id));
        await repo.AddAsync(Produto.Create("Camisa", 99m, categoriaB.Id));

        var resultado = await repo.GetByCategoriaAsync(categoriaA.Id);

        var lista = resultado.ToList();
        Assert.Equal(2, lista.Count);
        Assert.All(lista, p => Assert.Equal(categoriaA.Id, p.CategoriaId));
    }

    [Fact]
    public async Task Buscar_ComTermoPresenteNoNome_DeveRetornarProdutosCorrespondentes()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var categoria = await CriarCategoriaAsync(ctx);
        var repo = new ProdutoRepository(ctx);
        await repo.AddAsync(Produto.Create("Notebook Gamer", 4999m, categoria.Id));
        await repo.AddAsync(Produto.Create("Notebook Básico", 2999m, categoria.Id));
        await repo.AddAsync(Produto.Create("Monitor 4K", 1500m, categoria.Id));

        var resultado = await repo.SearchAsync("Notebook");

        var lista = resultado.ToList();
        Assert.Equal(2, lista.Count);
        Assert.All(lista, p => Assert.Contains("Notebook", p.Nome));
    }

    [Fact]
    public async Task Buscar_ComTermoNaoEncontrado_DeveRetornarListaVazia()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var categoria = await CriarCategoriaAsync(ctx);
        var repo = new ProdutoRepository(ctx);
        await repo.AddAsync(Produto.Create("Notebook", 2999m, categoria.Id));

        var resultado = await repo.SearchAsync("inexistente");

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task Atualizar_DeveModificarDadosDoProduto()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var categoria = await CriarCategoriaAsync(ctx);
        var repo = new ProdutoRepository(ctx);
        var produto = Produto.Create("Notebook", 2999m, categoria.Id);
        await repo.AddAsync(produto);

        produto.Update("Notebook Pro", 4999m, categoria.Id, "Nova desc");
        await repo.UpdateAsync(produto);

        var atualizado = await repo.GetByIdAsync(produto.Id);
        Assert.NotNull(atualizado);
        Assert.Equal("Notebook Pro", atualizado.Nome);
        Assert.Equal(4999m, atualizado.Preco);
    }

    [Fact]
    public async Task Reservar_DeveAtualizarStatusNoBancoDeDados()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var categoria = await CriarCategoriaAsync(ctx);
        var repo = new ProdutoRepository(ctx);
        var produto = Produto.Create("Notebook", 2999m, categoria.Id);
        await repo.AddAsync(produto);

        produto.Reservar();
        await repo.UpdateAsync(produto);

        var atualizado = await repo.GetByIdAsync(produto.Id);
        Assert.NotNull(atualizado);
        Assert.Equal(ProdutoStatus.Reservado, atualizado.Status);
    }

    [Fact]
    public async Task Remover_DeveExcluirProdutoDoContexto()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var categoria = await CriarCategoriaAsync(ctx);
        var repo = new ProdutoRepository(ctx);
        var produto = Produto.Create("Notebook", 2999m, categoria.Id);
        await repo.AddAsync(produto);

        await repo.DeleteAsync(produto);

        var removido = await repo.GetByIdAsync(produto.Id);
        Assert.Null(removido);
    }
}
