using CatalogoService.Application.Interfaces;
using Messaging.Contracts;
using MassTransit;

namespace CatalogoService.Infrastructure.Messaging.Consumers;

public class PedidoCanceladoConsumer(IProdutoApplicationService produtoService)
    : IConsumer<PedidoCanceladoEvento>
{
    public async Task Consume(ConsumeContext<PedidoCanceladoEvento> context)
    {
        foreach (var produtoId in context.Message.ProdutoIds)
            await produtoService.DisponibilizarAsync(produtoId, context.CancellationToken);
    }
}
