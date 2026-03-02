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
}
