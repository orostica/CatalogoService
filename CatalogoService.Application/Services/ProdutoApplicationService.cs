using CatalogoService.Application.DTOs;
using CatalogoService.Application.Interfaces;
using CatalogoService.Domain.Entities;
using CatalogoService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogoService.Application.Services
{
    public class ProdutoApplicationService : IProdutoApplicationService
    {
        private readonly IProdutoRepository _produtoRepository;

        public ProdutoApplicationService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }
        public async Task<IEnumerable<ProdutoDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var produtos = await _produtoRepository.GetAllAsync(cancellationToken);
            return produtos.Select(MapToDto);
        }

        public async Task<ProdutoDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var produto = await _produtoRepository.GetByIdAsync(id, cancellationToken);
            return produto is null ? null : MapToDto(produto);
        }

        public async Task<IEnumerable<ProdutoDto>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var produtos = await _produtoRepository.GetByCategoriaAsync(categoryId, cancellationToken);
            return produtos.Select(MapToDto);
        }

        public async Task<IEnumerable<ProdutoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var produtos = await _produtoRepository.SearchAsync(searchTerm, cancellationToken);
            return produtos.Select(MapToDto);
        }

        public async Task<ProdutoDto> CreateAsync(CriarProdutoDto dto, CancellationToken cancellationToken = default)
        {
            var produto = Produto.Create(
                dto.Nome,
                dto.Preco,
                dto.CategoriaId,
                dto.Descricao,
                dto.CoverImageUrl);

            await _produtoRepository.AddAsync(produto, cancellationToken);
            return MapToDto(produto);
        }

        public async Task<ProdutoDto?> UpdateAsync(Guid id, AtualizarProdutoDto dto, CancellationToken cancellationToken = default)
        {
            var produto = await _produtoRepository.GetByIdAsync(id, cancellationToken);
            if (produto is null) return null;

            produto.Update(
                dto.Nome,
                dto.Preco,
                dto.CategoriaId,
                dto.Descricao,
                dto.CoverImageUrl);

            await _produtoRepository.UpdateAsync(produto, cancellationToken);
            return MapToDto(produto);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var produto = await _produtoRepository.GetByIdAsync(id, cancellationToken);
            if (produto is null) return false;

            await _produtoRepository.DeleteAsync(produto, cancellationToken);
            return true;
        }
        private static ProdutoDto MapToDto(Produto produto) =>
            new(produto.Id,
                produto.Nome,
                produto.Descricao,
                produto.Preco,
                produto.ImagemUrl,
                produto.Status,
                produto.CategoriaId,
                produto.Categoria?.Nome ?? string.Empty,
                produto.CriadoEm,
                produto.AtualizadoEm);
    }
}
