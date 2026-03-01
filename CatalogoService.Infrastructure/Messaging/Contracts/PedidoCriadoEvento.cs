namespace Messaging.Contracts;

public record PedidoCriadoEvento(
    Guid PedidoId,
    Guid ClienteId,
    List<ItemPedidoEvento> Itens,
    decimal ValorTotal,
    DateTime CriadoEm);

public record ItemPedidoEvento(
    Guid ProdutoId,
    int Quantidade,
    decimal PrecoUnitario);
