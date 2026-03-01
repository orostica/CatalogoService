using CatalogoService.Application.Interfaces;
using Messaging.Contracts;
using MassTransit;

namespace CatalogoService.Infrastructure.Messaging.Consumers;

public class PedidoConfirmadoConsumer(IProdutoApplicationService produtoService)
    : IConsumer<PedidoConfirmadoEvento>
{
    public async Task Consume(ConsumeContext<PedidoConfirmadoEvento> context)
    {
        foreach (var produtoId in context.Message.ProdutoIds)
            await produtoService.IndisponibilizarAsync(produtoId, context.CancellationToken);
    }
}
