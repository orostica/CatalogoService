namespace Messaging.Contracts;

public record PedidoCanceladoEvento(
    Guid PedidoId,
    IReadOnlyList<Guid> ProdutoIds);
