using CatalogoService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatalogoService.IntegrationTests.Fixtures;

public static class CatalogoDbContextFactory
{
    public static CatalogoDbContext CriarContexto(string? nomeBanco = null)
    {
        var opcoes = new DbContextOptionsBuilder<CatalogoDbContext>()
            .UseInMemoryDatabase(nomeBanco ?? Guid.NewGuid().ToString())
            .Options;

        return new CatalogoDbContext(opcoes);
    }
}
