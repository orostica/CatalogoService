using System;
using System.Collections.Generic;
using System.Text;
using CatalogoService.Domain.Enums;

namespace CatalogoService.Application.DTOs;
public record ProdutoDto(
    Guid Id,
    string Nome,
    string? Descricao,
    decimal Preco,
    string? ImagemUrl,
    ProdutoStatus Status,
    Guid CategoriaId,
    string CategoriaNome,
    DateTime CriadoEm,
    DateTime AtualizadoEm);

public record CriarProdutoDto(
    Guid Id,
    string Nome,
    string? Descricao,
    decimal Preco,
    string? CoverImageUrl,
    Guid CategoriaId);

public record AtualizarProdutoDto(
    string Nome,
    string? Descricao,
    decimal Preco,
    string? ImagemUrl,
    Guid CategoriaId,
    string? CoverImageUrl);
