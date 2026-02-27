using Autofac;
using CatalogoService.Application.Interfaces;
using CatalogoService.Application.Services;
using CatalogoService.Domain.Interfaces;
using CatalogoService.Infrastructure.Repositories;

namespace CatalogService.API;

public class IocConfiguration : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Application — Services
        builder.RegisterType<ProdutoApplicationService>().As<IProdutoApplicationService>().InstancePerLifetimeScope();
        builder.RegisterType<CategoriaApplicationService>().As<ICategoriaApplicationService>().InstancePerLifetimeScope();

        // Infrastructure — Repositories
        builder.RegisterType<ProdutoRepository>().As<IProdutoRepository>().InstancePerLifetimeScope();
        builder.RegisterType<CategoriaRepository>().As<ICategoriaRepository>().InstancePerLifetimeScope();
    }
}
