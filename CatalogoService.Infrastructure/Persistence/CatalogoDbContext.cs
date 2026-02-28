using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using CatalogoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatalogoService.Infrastructure.Persistence
{
    public class CatalogoDbContext(DbContextOptions<CatalogoDbContext> options) : DbContext(options)
    {
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Categoria> Categorias => Set<Categoria>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogoDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }

}
