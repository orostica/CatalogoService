namespace Messaging.Contracts;

public record ProdutosReservadosEvento(
    Guid PedidoId,
    bool Sucesso,
    string? MotivoFalha,
    List<Guid> ProdutosReservados);
