using CatalogoService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogoService.Domain.Interfaces
{
    public interface IProdutoRepository : IRepository<Produto>
    {
        Task<IEnumerable<Produto>> GetByCategoriaAsync(Guid categoriaId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Produto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
}
