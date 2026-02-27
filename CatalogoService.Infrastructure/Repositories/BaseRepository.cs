using CatalogoService.Domain.Common;
using CatalogoService.Domain.Interfaces;
using CatalogoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogoService.Infrastructure.Repositories
{
    public abstract class BaseRepository<T>(CatalogoDbContext context) : IRepository<T>
        where T : BaseEntity
    {
        protected readonly CatalogoDbContext Context = context;
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await DbSet.FindAsync([id], cancellationToken);

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await DbSet.AsNoTracking().ToListAsync(cancellationToken);

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
            => await DbSet.AnyAsync(e => e.Id == id, cancellationToken);
    }
}
