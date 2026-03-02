using CatalogoService.Application.DTOs;
using CatalogoService.Application.Services;
using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Interfaces;
using Moq;
using Xunit;

namespace CatalogoService.UnitTests.Application;

public class CategoriaApplicationServiceTests
{
    private readonly Mock<ICategoriaRepository> _repositorioMock;
    private readonly CategoriaApplicationService _servico;

    public CategoriaApplicationServiceTests()
    {
        _repositorioMock = new Mock<ICategoriaRepository>();
        _servico = new CategoriaApplicationService(_repositorioMock.Object);
    }

    [Fact]
    public async Task Criar_ComDadosValidos_DeveChamarAddERetornarDto()
    {
        var dto = new CriarCategoriaDto("Eletrônicos", "Produtos eletrônicos");
        _repositorioMock.Setup(r => r.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.CreateAsync(dto);

        Assert.NotNull(resultado);
        Assert.Equal("Eletrônicos", resultado.Nome);
        Assert.Equal("Produtos eletrônicos", resultado.Descricao);
        Assert.NotEqual(Guid.Empty, resultado.Id);
        _repositorioMock.Verify(r => r.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Atualizar_QuandoCategoriaExiste_DeveAtualizarERetornarDto()
    {
        var categoria = Categoria.Create("Eletrônicos");
        var dto = new AtualizarCategoriaDto("Informática", "Computadores");
        _repositorioMock.Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);
        _repositorioMock.Setup(r => r.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.UpdateAsync(categoria.Id, dto);

        Assert.NotNull(resultado);
        Assert.Equal("Informática", resultado.Nome);
        Assert.Equal("Computadores", resultado.Descricao);
        _repositorioMock.Verify(r => r.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Atualizar_QuandoCategoriaNaoExiste_DeveRetornarNull()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);

        var resultado = await _servico.UpdateAsync(Guid.NewGuid(), new AtualizarCategoriaDto("X", null));

        Assert.Null(resultado);
        _repositorioMock.Verify(r => r.UpdateAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
