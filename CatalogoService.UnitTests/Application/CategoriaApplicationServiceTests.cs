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
    public async Task ObterTodos_QuandoExistemCategorias_DeveRetornarListaDeDtos()
    {
        var categorias = new List<Categoria>
        {
            Categoria.Create("Eletrônicos", "Desc 1"),
            Categoria.Create("Informática", "Desc 2")
        };
        _repositorioMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        var resultado = await _servico.GetAllAsync();

        var lista = resultado.ToList();
        Assert.Equal(2, lista.Count);
        Assert.Contains(lista, d => d.Nome == "Eletrônicos");
        Assert.Contains(lista, d => d.Nome == "Informática");
    }

    [Fact]
    public async Task ObterTodos_QuandoNaoExistemCategorias_DeveRetornarListaVazia()
    {
        _repositorioMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Categoria>());

        var resultado = await _servico.GetAllAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObterPorId_QuandoCategoriaExiste_DeveRetornarDto()
    {
        var categoria = Categoria.Create("Eletrônicos", "Desc");
        _repositorioMock.Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);

        var resultado = await _servico.GetByIdAsync(categoria.Id);

        Assert.NotNull(resultado);
        Assert.Equal(categoria.Id, resultado.Id);
        Assert.Equal("Eletrônicos", resultado.Nome);
    }

    [Fact]
    public async Task ObterPorId_QuandoCategoriaNaoExiste_DeveRetornarNull()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);

        var resultado = await _servico.GetByIdAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    [Fact]
    public async Task Buscar_ComTermoValido_DeveRepassarTermoAoRepositorioERetornarDtos()
    {
        const string termo = "elet";
        var categorias = new List<Categoria> { Categoria.Create("Eletrônicos") };
        _repositorioMock.Setup(r => r.SearchAsync(termo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categorias);

        var resultado = await _servico.SearchAsync(termo);

        var lista = resultado.ToList();
        Assert.Single(lista);
        Assert.Equal("Eletrônicos", lista[0].Nome);
        _repositorioMock.Verify(r => r.SearchAsync(termo, It.IsAny<CancellationToken>()), Times.Once);
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

    [Fact]
    public async Task Remover_QuandoCategoriaExiste_DeveChamarDeleteERetornarTrue()
    {
        var categoria = Categoria.Create("Eletrônicos");
        _repositorioMock.Setup(r => r.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categoria);
        _repositorioMock.Setup(r => r.DeleteAsync(categoria, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.DeleteAsync(categoria.Id);

        Assert.True(resultado);
        _repositorioMock.Verify(r => r.DeleteAsync(categoria, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Remover_QuandoCategoriaNaoExiste_DeveRetornarFalse()
    {
        _repositorioMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Categoria?)null);

        var resultado = await _servico.DeleteAsync(Guid.NewGuid());

        Assert.False(resultado);
        _repositorioMock.Verify(r => r.DeleteAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Criar_DeveMapearTodasAsPropriedadesParaDto()
    {
        var dto = new CriarCategoriaDto("Eletrônicos", "Desc");
        _repositorioMock.Setup(r => r.AddAsync(It.IsAny<Categoria>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resultado = await _servico.CreateAsync(dto);

        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.Equal("Eletrônicos", resultado.Nome);
        Assert.Equal("Desc", resultado.Descricao);
        Assert.NotEqual(default, resultado.CriadoEm);
        Assert.NotEqual(default, resultado.AtualizadoEm);
    }
}
