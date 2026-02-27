using CatalogoService.Application.DTOs;
using CatalogoService.Application.Interfaces;
using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Interfaces;

namespace CatalogoService.Application.Services
{
    public class CategoriaApplicationService : ICategoriaApplicationService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaApplicationService(ICategoriaRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<CategoriaDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var categorias = await _repository.GetAllAsync(cancellationToken);
            return categorias.Select(MapToDto);
        }

        public async Task<CategoriaDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var categoria = await _repository.GetByIdAsync(id, cancellationToken);
            return categoria is null ? null : MapToDto(categoria);
        }
        public async Task<IEnumerable<CategoriaDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var categorias = await _repository.SearchAsync(searchTerm, cancellationToken);
            return categorias.Select(MapToDto);
        }

        public async Task<CategoriaDto> CreateAsync(CriarCategoriaDto dto, CancellationToken cancellationToken = default)
        {
            var categoria = Categoria.Create(dto.Nome, dto.Descricao);
            await _repository.AddAsync(categoria, cancellationToken);
            return MapToDto(categoria);
        }

        public async Task<CategoriaDto?> UpdateAsync(Guid id, AtualizarCategoriaDto dto, CancellationToken cancellationToken = default)
        {
            var categoria = await _repository.GetByIdAsync(id, cancellationToken);
            if (categoria is null) return null;

            categoria.Update(dto.Nome, dto.Descricao);
            await _repository.UpdateAsync(categoria, cancellationToken);
            return MapToDto(categoria);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var categoria = await _repository.GetByIdAsync(id, cancellationToken);
            if (categoria is null) return false;

            await _repository.DeleteAsync(categoria, cancellationToken);
            return true;
        }

        private static CategoriaDto MapToDto(Categoria categoria) =>
            new(categoria.Id, categoria.Nome, categoria.Descricao, categoria.CriadoEm, categoria.AtualizadoEm);
    }
}
