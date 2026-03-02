using CatalogoService.Application.DTOs;
using CatalogoService.Application.Services;
using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Enums;
using CatalogoService.Domain.Interfaces;
using Moq;
using Xunit;

namespace CatalogoService.UnitTests.Application;

public class ProdutoApplicationServiceTests
{
    private readonly Mock<IProdutoRepository> _repositorioMock;
    private readonly ProdutoApplicationService _servico;
    private static readonly Guid CategoriaId = Guid.NewGuid();

    public ProdutoApplicationServiceTests()
    {
        _repositorioMock = new Mock<IProdutoRepository>();
        _servico = new ProdutoApplicationService(_repositorioMock.Object);
    }

    private static Produto CriarProduto(string nome = "Notebook", decimal preco = 2999.99m)
        => Produto.Create(nome, preco, CategoriaId, "Descrição", "http://img.com/nb.jpg");

    [Fact]
    public async Task ObterTodos_QuandoExistemProdutos_DeveRetornarListaDeDtos()
    {
        var produtos = new List<Produto>
        {
            CriarProduto("Notebook"),
            CriarProduto("Monitor")
        };
        _repositorioMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(produtos);

        var resultado = await _servico.GetAllAsync();

        var lista = resultado.ToList();
        Assert.Equal(2, lista.Count);
        Assert.Contains(lista, d => d.Nome == "Notebook");
        Assert.Contains(lista, d => d.Nome == "Monitor");
    }

    [Fact]
    public async Task ObterTodos_QuandoNaoExistemProdutos_DeveRetornarListaVazia()
    {
        _repositorioMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Produto>());

        var resultado = await _servico.GetAllAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObterPorId_QuandoProdutoExiste_DeveRetornarDto()
    {
        var produto = CriarProduto();
        _repositorioMock.Setup(r => r.GetByIdAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);

        var resultado = await _servico.GetByIdAsync(produto.Id);

        Assert.NotNull(resultado);
        Assert.Equal(produto.Id, resultado.Id);
        Assert.Equal("Notebook", resultado.Nome);
        Assert.Equal(ProdutoStatus.Disponivel, resultado.Status);
    }

    [Fact]
    public async Task ObterPorId_QuandoProdutoNaoExiste_DeveRetornarNull()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        var resultado = await _servico.GetByIdAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    [Fact]
    public async Task ObterPorCategoria_DeveRepassarCategoriaIdERetornarDtos()
    {
        var produtos = new List<Produto> { CriarProduto() };
        _repositorioMock.Setup(r => r.GetByCategoriaAsync(CategoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produtos);

        var resultado = await _servico.GetByCategoryAsync(CategoriaId);

        Assert.Single(resultado);
        _repositorioMock.Verify(r => r.GetByCategoriaAsync(CategoriaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Buscar_ComTermoValido_DeveRetornarProdutosEncontrados()
    {
        const string termo = "note";
        var produtos = new List<Produto> { CriarProduto() };
        _repositorioMock.Setup(r => r.SearchAsync(termo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produtos);

        var resultado = await _servico.SearchAsync(termo);

        Assert.Single(resultado);
        _repositorioMock.Verify(r => r.SearchAsync(termo, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Criar_ComDadosValidos_DeveChamarAddERetornarDto()
    {
        var dto = new CriarProdutoDto(Guid.NewGuid(), "Notebook", "Desc", 2999.99m, "http://img.com", CategoriaId);
        _repositorioMock.Setup(r => r.AddAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.CreateAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal("Notebook", resultado.Nome);
        Assert.Equal(2999.99m, resultado.Preco);
        Assert.Equal(ProdutoStatus.Disponivel, resultado.Status);
        _repositorioMock.Verify(r => r.AddAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Atualizar_QuandoProdutoExiste_DeveAtualizarERetornarDto()
    {
        var produto = CriarProduto();
        var dto = new AtualizarProdutoDto("Notebook Pro", "Nova desc", 4999.99m, "http://nova.jpg", CategoriaId, null);
        _repositorioMock.Setup(r => r.GetByIdAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _repositorioMock.Setup(r => r.UpdateAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.UpdateAsync(produto.Id, dto);

        Assert.NotNull(resultado);
        Assert.Equal("Notebook Pro", resultado.Nome);
        Assert.Equal(4999.99m, resultado.Preco);
        _repositorioMock.Verify(r => r.UpdateAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Atualizar_QuandoProdutoNaoExiste_DeveRetornarNull()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        var resultado = await _servico.UpdateAsync(Guid.NewGuid(), new AtualizarProdutoDto("X", null, 1m, null, CategoriaId, null));

        Assert.Null(resultado);
        _repositorioMock.Verify(r => r.UpdateAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Remover_QuandoProdutoExiste_DeveChamarDeleteERetornarTrue()
    {
        var produto = CriarProduto();
        _repositorioMock.Setup(r => r.GetByIdAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _repositorioMock.Setup(r => r.DeleteAsync(produto, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.DeleteAsync(produto.Id);

        Assert.True(resultado);
        _repositorioMock.Verify(r => r.DeleteAsync(produto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Remover_QuandoProdutoNaoExiste_DeveRetornarFalse()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        var resultado = await _servico.DeleteAsync(Guid.NewGuid());

        Assert.False(resultado);
    }

    [Fact]
    public async Task Reservar_QuandoProdutoExiste_DeveAlterarStatusERetornarTrue()
    {
        var produto = CriarProduto();
        _repositorioMock.Setup(r => r.GetByIdAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _repositorioMock.Setup(r => r.UpdateAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.ReservarAsync(produto.Id);

        Assert.True(resultado);
        Assert.Equal(ProdutoStatus.Reservado, produto.Status);
        _repositorioMock.Verify(r => r.UpdateAsync(produto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Reservar_QuandoProdutoNaoExiste_DeveRetornarFalse()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        var resultado = await _servico.ReservarAsync(Guid.NewGuid());

        Assert.False(resultado);
        _repositorioMock.Verify(r => r.UpdateAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Indisponibilizar_QuandoProdutoExiste_DeveAlterarStatusERetornarTrue()
    {
        var produto = CriarProduto();
        _repositorioMock.Setup(r => r.GetByIdAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _repositorioMock.Setup(r => r.UpdateAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.IndisponibilizarAsync(produto.Id);

        Assert.True(resultado);
        Assert.Equal(ProdutoStatus.Indisponivel, produto.Status);
    }

    [Fact]
    public async Task Indisponibilizar_QuandoProdutoNaoExiste_DeveRetornarFalse()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        var resultado = await _servico.IndisponibilizarAsync(Guid.NewGuid());

        Assert.False(resultado);
    }

    [Fact]
    public async Task Disponibilizar_QuandoProdutoExiste_DeveAlterarStatusERetornarTrue()
    {
        var produto = CriarProduto();
        produto.Indisponibilizar();
        _repositorioMock.Setup(r => r.GetByIdAsync(produto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(produto);
        _repositorioMock.Setup(r => r.UpdateAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.DisponibilizarAsync(produto.Id);

        Assert.True(resultado);
        Assert.Equal(ProdutoStatus.Disponivel, produto.Status);
    }

    [Fact]
    public async Task Disponibilizar_QuandoProdutoNaoExiste_DeveRetornarFalse()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Produto?)null);

        var resultado = await _servico.DisponibilizarAsync(Guid.NewGuid());

        Assert.False(resultado);
    }
}
