using CatalogoService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogoService.Domain.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<IEnumerable<Categoria>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
}
