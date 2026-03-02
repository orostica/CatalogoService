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
}
