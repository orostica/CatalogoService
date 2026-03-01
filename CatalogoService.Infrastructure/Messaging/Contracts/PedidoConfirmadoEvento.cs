namespace Messaging.Contracts;

public record PedidoConfirmadoEvento(
    Guid PedidoId,
    IReadOnlyList<Guid> ProdutoIds);
