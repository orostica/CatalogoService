using CatalogoService.Application.Interfaces;
using Messaging.Contracts;
using MassTransit;

namespace CatalogoService.Infrastructure.Messaging.Consumers;

public class PedidoCriadoConsumer(
    IProdutoApplicationService produtoService,
    IBus bus) : IConsumer<PedidoCriadoEvento>
{
    public async Task Consume(ConsumeContext<PedidoCriadoEvento> context)
    {
        var evento = context.Message;
        var produtosReservados = new List<Guid>();
        string? motivoFalha = null;

        foreach (var item in evento.Itens)
        {
            var produto = await produtoService.GetByIdAsync(item.ProdutoId, context.CancellationToken);

            if (produto is null)
            {
                motivoFalha = $"Produto {item.ProdutoId} não encontrado.";
                break;
            }

            await produtoService.ReservarAsync(item.ProdutoId, context.CancellationToken);
            produtosReservados.Add(item.ProdutoId);
        }

        var sucesso = motivoFalha is null;

        await bus.Publish(new ProdutosReservadosEvento(
            evento.PedidoId,
            sucesso,
            motivoFalha,
            produtosReservados), context.CancellationToken);
    }
}
