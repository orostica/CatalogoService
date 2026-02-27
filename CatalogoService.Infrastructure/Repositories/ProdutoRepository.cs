using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Interfaces;
using CatalogoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogoService.Infrastructure.Repositories
{
    public class ProdutoRepository(CatalogoDbContext context) : BaseRepository<Produto>(context), IProdutoRepository
    {
        public override async Task<Produto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
       => await DbSet
           .Include(b => b.Categoria)
           .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        public override async Task<IEnumerable<Produto>> GetAllAsync(CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .Include(b => b.Categoria)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Produto>> GetByCategoriaAsync(Guid categoryId, CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .Include(b => b.Categoria)
                .Where(b => b.CategoriaId == categoryId)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Produto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .Include(b => b.Categoria)
                .Where(b => b.Nome.Contains(searchTerm) || b.Descricao.Contains(searchTerm))
                .ToListAsync(cancellationToken);
    }
}
