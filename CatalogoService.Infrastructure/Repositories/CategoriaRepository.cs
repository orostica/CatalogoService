using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Interfaces;
using CatalogoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogoService.Infrastructure.Repositories
{
    internal class CategoriaRepository(CatalogoDbContext context) : BaseRepository<Categoria>(context), ICategoriaRepository
    {
        public async Task<IEnumerable<Categoria>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
            => await DbSet
                .AsNoTracking()
                .Where(b => b.Nome.Contains(searchTerm) || b.Descricao.Contains(searchTerm))
                .ToListAsync(cancellationToken);
    }
}
