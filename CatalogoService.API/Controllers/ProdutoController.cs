using CatalogoService.Application.DTOs;
using CatalogoService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatalogoService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProdutoController(IProdutoApplicationService produtoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var produtos = await produtoService.GetAllAsync(cancellationToken);
        return Ok(produtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var produto = await produtoService.GetByIdAsync(id, cancellationToken);
        return produto is null ? NotFound() : Ok(produto);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term, CancellationToken cancellationToken)
    {
        var produtos = await produtoService.SearchAsync(term, cancellationToken);
        return Ok(produtos);
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetByCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var produtos = await produtoService.GetByCategoryAsync(categoryId, cancellationToken);
        return Ok(produtos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarProdutoDto dto, CancellationToken cancellationToken)
    {
        var produto = await produtoService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = produto.Id }, produto);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AtualizarProdutoDto dto, CancellationToken cancellationToken)
    {
        var produto = await produtoService.UpdateAsync(id, dto, cancellationToken);
        return produto is null ? NotFound() : Ok(produto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await produtoService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
