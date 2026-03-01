using CatalogoService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogoService.Application.Interfaces
{
    public interface IProdutoApplicationService
    {
        Task<IEnumerable<ProdutoDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProdutoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProdutoDto>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProdutoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<ProdutoDto> CreateAsync(CriarProdutoDto dto, CancellationToken cancellationToken = default);
        Task<ProdutoDto?> UpdateAsync(Guid id, AtualizarProdutoDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ReservarAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> IndisponibilizarAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DisponibilizarAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
