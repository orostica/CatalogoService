using CatalogoService.Domain.Entities;
using CatalogoService.Infrastructure.Repositories;
using CatalogoService.IntegrationTests.Fixtures;
using Xunit;

namespace CatalogoService.IntegrationTests.Repositories;

public class CategoriaRepositoryTests
{
    [Fact]
    public async Task Adicionar_DeveInserirCategoriaNoContexto()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        var categoria = Categoria.Create("Eletrônicos", "Desc");

        await repo.AddAsync(categoria);

        var salva = await repo.GetByIdAsync(categoria.Id);
        Assert.NotNull(salva);
        Assert.Equal("Eletrônicos", salva.Nome);
    }

    [Fact]
    public async Task ObterPorId_QuandoExiste_DeveRetornarCategoria()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        var categoria = Categoria.Create("Informática");
        await repo.AddAsync(categoria);

        var resultado = await repo.GetByIdAsync(categoria.Id);

        Assert.NotNull(resultado);
        Assert.Equal(categoria.Id, resultado.Id);
        Assert.Equal("Informática", resultado.Nome);
    }

    [Fact]
    public async Task ObterPorId_QuandoNaoExiste_DeveRetornarNull()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);

        var resultado = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarTodasAsCategorias()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        await repo.AddAsync(Categoria.Create("Eletrônicos"));
        await repo.AddAsync(Categoria.Create("Informática"));
        await repo.AddAsync(Categoria.Create("Vestuário"));

        var resultado = await repo.GetAllAsync();

        Assert.Equal(3, resultado.Count());
    }

    [Fact]
    public async Task Atualizar_DeveModificarDadosDaCategoria()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        var categoria = Categoria.Create("Eletrônicos", "Descrição antiga");
        await repo.AddAsync(categoria);

        categoria.Update("Eletrodomésticos", "Descrição nova");
        await repo.UpdateAsync(categoria);

        var atualizada = await repo.GetByIdAsync(categoria.Id);
        Assert.NotNull(atualizada);
        Assert.Equal("Eletrodomésticos", atualizada.Nome);
        Assert.Equal("Descrição nova", atualizada.Descricao);
    }

    [Fact]
    public async Task Remover_DeveExcluirCategoriaDoContexto()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        var categoria = Categoria.Create("Eletrônicos");
        await repo.AddAsync(categoria);

        await repo.DeleteAsync(categoria);

        var removida = await repo.GetByIdAsync(categoria.Id);
        Assert.Null(removida);
    }

    [Fact]
    public async Task ExisteAsync_QuandoExiste_DeveRetornarTrue()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        var categoria = Categoria.Create("Eletrônicos");
        await repo.AddAsync(categoria);

        var existe = await repo.ExistsAsync(categoria.Id);

        Assert.True(existe);
    }

    [Fact]
    public async Task ExisteAsync_QuandoNaoExiste_DeveRetornarFalse()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);

        var existe = await repo.ExistsAsync(Guid.NewGuid());

        Assert.False(existe);
    }

    [Fact]
    public async Task Buscar_ComTermoPresenteNaDescricao_DeveRetornarCategoriasCorrespondentes_Multiplas()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        await repo.AddAsync(Categoria.Create("Categoria A", "Produto notebook portatil avancado"));
        await repo.AddAsync(Categoria.Create("Categoria B", "Produto notebook basico portatil"));
        await repo.AddAsync(Categoria.Create("Categoria C", "Produto sem relacao alguma"));

        var resultado = await repo.SearchAsync("notebook");

        var lista = resultado.ToList();
        Assert.Equal(2, lista.Count);
        Assert.All(lista, c => Assert.Contains("notebook", c.Descricao));
    }

    [Fact]
    public async Task Buscar_ComTermoPresenteNaDescricao_DeveRetornarCategoriasCorrespondentes()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        await repo.AddAsync(Categoria.Create("Eletrônicos", "Dispositivos com bateria"));
        await repo.AddAsync(Categoria.Create("Informática", "Computadores e acessórios"));

        var resultado = await repo.SearchAsync("bateria");

        var lista = resultado.ToList();
        Assert.Single(lista);
        Assert.Equal("Eletrônicos", lista[0].Nome);
    }

    [Fact]
    public async Task Buscar_ComTermoNaoEncontrado_DeveRetornarListaVazia()
    {
        await using var ctx = CatalogoDbContextFactory.CriarContexto();
        var repo = new CategoriaRepository(ctx);
        await repo.AddAsync(Categoria.Create("Eletrônicos"));

        var resultado = await repo.SearchAsync("inexistente");

        Assert.Empty(resultado);
    }
}
