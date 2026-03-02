using CatalogoService.Application.Interfaces;
using CatalogoService.Infrastructure.Messaging.Consumers;
using MassTransit;
using Messaging.Contracts;
using Moq;
using Xunit;

namespace CatalogoService.UnitTests.Messaging;

public class PedidoCanceladoConsumerTests
{
    private readonly Mock<IProdutoApplicationService> _produtoServiceMock;
    private readonly PedidoCanceladoConsumer _consumer;

    public PedidoCanceladoConsumerTests()
    {
        _produtoServiceMock = new Mock<IProdutoApplicationService>();
        _consumer = new PedidoCanceladoConsumer(_produtoServiceMock.Object);
    }

    private static Mock<ConsumeContext<PedidoCanceladoEvento>> CriarContexto(PedidoCanceladoEvento evento)
    {
        var contexto = new Mock<ConsumeContext<PedidoCanceladoEvento>>();
        contexto.Setup(c => c.Message).Returns(evento);
        contexto.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return contexto;
    }

    [Fact]
    public async Task Consumir_ComUmProduto_DeveDisponibilizarEsseProduto()
    {
        var produtoId = Guid.NewGuid();
        var evento = new PedidoCanceladoEvento(Guid.NewGuid(), new List<Guid> { produtoId });

        _produtoServiceMock.Setup(s => s.DisponibilizarAsync(produtoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(CriarContexto(evento).Object);

        _produtoServiceMock.Verify(s => s.DisponibilizarAsync(produtoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consumir_ComMultiplosProdutos_DeveDisponibilizarTodosOsProdutos()
    {
        var produto1Id = Guid.NewGuid();
        var produto2Id = Guid.NewGuid();
        var evento = new PedidoCanceladoEvento(
            Guid.NewGuid(),
            new List<Guid> { produto1Id, produto2Id });

        _produtoServiceMock.Setup(s => s.DisponibilizarAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(CriarContexto(evento).Object);

        _produtoServiceMock.Verify(s => s.DisponibilizarAsync(produto1Id, It.IsAny<CancellationToken>()), Times.Once);
        _produtoServiceMock.Verify(s => s.DisponibilizarAsync(produto2Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consumir_SemProdutos_NaoDeveChamarDisponibilizar()
    {
        var evento = new PedidoCanceladoEvento(Guid.NewGuid(), new List<Guid>());

        await _consumer.Consume(CriarContexto(evento).Object);

        _produtoServiceMock.Verify(s => s.DisponibilizarAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
