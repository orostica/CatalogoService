using CatalogoService.Application.DTOs;
using CatalogoService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogoService.Application.Interfaces
{
    public interface ICategoriaApplicationService
    {
        Task<IEnumerable<CategoriaDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<CategoriaDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoriaDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<CategoriaDto> CreateAsync(CriarCategoriaDto dto, CancellationToken cancellationToken = default);
        Task<CategoriaDto?> UpdateAsync(Guid id, AtualizarCategoriaDto dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
