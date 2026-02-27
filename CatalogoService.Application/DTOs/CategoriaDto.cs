using System;
using System.Collections.Generic;
using System.Text;

namespace CatalogoService.Application.DTOs;
public record CategoriaDto(
    Guid Id,
    string Nome,
    string? Descricao,
    DateTime CriadoEm,
    DateTime AtualizadoEm);

public record CriarCategoriaDto(
    string Nome,
    string? Descricao);

public record AtualizarCategoriaDto(
    string Nome,
    string? Descricao);
