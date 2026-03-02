using CatalogoService.Application.DTOs;
using CatalogoService.Application.Interfaces;
using CatalogoService.Domain.Enums;
using CatalogoService.Infrastructure.Messaging.Consumers;
using MassTransit;
using Messaging.Contracts;
using Moq;
using Xunit;

namespace CatalogoService.UnitTests.Messaging;

public class PedidoCriadoConsumerTests
{
    private readonly Mock<IProdutoApplicationService> _produtoServiceMock;
    private readonly Mock<IBus> _busMock;
    private readonly PedidoCriadoConsumer _consumer;

    public PedidoCriadoConsumerTests()
    {
        _produtoServiceMock = new Mock<IProdutoApplicationService>();
        _busMock = new Mock<IBus>();
        _consumer = new PedidoCriadoConsumer(_produtoServiceMock.Object, _busMock.Object);
    }

    private static Mock<ConsumeContext<PedidoCriadoEvento>> CriarContexto(PedidoCriadoEvento evento)
    {
        var contexto = new Mock<ConsumeContext<PedidoCriadoEvento>>();
        contexto.Setup(c => c.Message).Returns(evento);
        contexto.Setup(c => c.CancellationToken).Returns(CancellationToken.None);
        return contexto;
    }

    [Fact]
    public async Task Consumir_QuandoTodosProdutosExistem_DeveReservarCadaUmEPublicarEventoComSucesso()
    {
        var produtoId = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var evento = new PedidoCriadoEvento(
            pedidoId,
            Guid.NewGuid(),
            new List<ItemPedidoEvento> { new(produtoId, 1, 100m) },
            100m,
            DateTime.UtcNow);

        _produtoServiceMock.Setup(s => s.GetByIdAsync(produtoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProdutoDto(produtoId, "Notebook", null, 100m, null,
                ProdutoStatus.Disponivel, Guid.NewGuid(), "Eletrônicos", DateTime.UtcNow, DateTime.UtcNow));

        _produtoServiceMock.Setup(s => s.ReservarAsync(produtoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _busMock.Setup(b => b.Publish(It.IsAny<ProdutosReservadosEvento>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _consumer.Consume(CriarContexto(evento).Object);

        _produtoServiceMock.Verify(s => s.ReservarAsync(produtoId, It.IsAny<CancellationToken>()), Times.Once);
        _busMock.Verify(b => b.Publish(
            It.Is<ProdutosReservadosEvento>(e => e.PedidoId == pedidoId && e.Sucesso),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Consumir_QuandoProdutoNaoEncontrado_DevePublicarEventoComFalha()
    {
        var produtoIdInexistente = Guid.NewGuid();
        var pedidoId = Guid.NewGuid();
        var evento = new PedidoCriadoEvento(
            pedidoId,
            Guid.NewGuid(),
            new List<ItemPedidoEvento> { new(produtoIdInexistente, 1, 100m) },
            100m,
            DateTime.UtcNow);

        _produtoServiceMock.Setup(s => s.GetByIdAsync(produtoIdInexistente, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProdutoDto?)null);

        _busMock.Setup(b => b.Publish(It.IsAny<ProdutosReservadosEvento>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _consumer.Consume(CriarContexto(evento).Object);

        _produtoServiceMock.Verify(s => s.ReservarAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _busMock.Verify(b => b.Publish(
            It.Is<ProdutosReservadosEvento>(e => e.PedidoId == pedidoId && !e.Sucesso && e.MotivoFalha != null),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
