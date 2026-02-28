using CatalogoService.Application.DTOs;
using CatalogoService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriaController(ICategoriaApplicationService categoriaService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var categorias = await categoriaService.GetAllAsync(cancellationToken);
        return Ok(categorias);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var categoria = await categoriaService.GetByIdAsync(id, cancellationToken);
        return categoria is null ? NotFound() : Ok(categoria);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, CancellationToken cancellationToken)
    {
        var categorias = await categoriaService.SearchAsync(term, cancellationToken);
        return Ok(categorias);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarCategoriaDto dto, CancellationToken cancellationToken)
    {
        var categoria = await categoriaService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarCategoriaDto dto, CancellationToken cancellationToken)
    {
        var categoria = await categoriaService.UpdateAsync(id, dto, cancellationToken);
        return categoria is null ? NotFound() : Ok(categoria);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deletado = await categoriaService.DeleteAsync(id, cancellationToken);
        return deletado ? NoContent() : NotFound();
    }
}
