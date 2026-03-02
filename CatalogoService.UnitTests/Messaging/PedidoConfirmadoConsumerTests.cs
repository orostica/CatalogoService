using CatalogoService.Application.Interfaces;
using CatalogoService.Infrastructure.Messaging.Consumers;
using MassTransit;
using Messaging.Contracts;
using Moq;
using Xunit;

namespace CatalogoService.UnitTests.Messaging;

public class PedidoConfirmadoConsumerTests
{
    private readonly Mock<IProdutoApplicationService> _produtoServiceMock;
    private readonly PedidoConfirmadoConsumer _consumer;

    public PedidoConfirmadoConsumerTests()
    {
        _produtoServiceMock = new Mock<IProdutoApplicationService>();
        _consumer = new PedidoConfirmadoConsumer(_produtoServiceMock.Object);
    }

    private static Mock<ConsumeContext<PedidoConfirmadoEvento>> CriarContexto(PedidoConfirmadoEvento evento)
    {
        var contexto = new Mock<ConsumeContext<PedidoConfirmadoEvento>>();
        contexto.Setup(c => c.Message).Returns(evento);
        contexto.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return contexto;
    }

    [Fact]
    public async Task Consumir_ComUmProduto_DeveIndisponibilizarEsseProduto()
    {
        var produtoId = Guid.NewGuid();
        var evento = new PedidoConfirmadoEvento(Guid.NewGuid(), new List<Guid> { produtoId });

        _produtoServiceMock.Setup(s => s.IndisponibilizarAsync(produtoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _consumer.Consume(CriarContexto(evento).Object);

        _produtoServiceMock.Verify(s => s.IndisponibilizarAsync(produtoId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
