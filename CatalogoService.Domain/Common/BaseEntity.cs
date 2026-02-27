namespace CatalogoService.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
        public DateTime AtualizadoEm { get; protected set; } = DateTime.UtcNow;

        public void SetAtualizadoEm() => AtualizadoEm = DateTime.UtcNow;
    }
}
