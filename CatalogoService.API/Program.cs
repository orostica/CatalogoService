using Autofac;
using Autofac.Extensions.DependencyInjection;
using CatalogService.API; // onde está o IocConfiguration
using CatalogoService.Infrastructure.Messaging.Consumers;
using Messaging.Contracts;
using CatalogoService.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new IocConfiguration());
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddDbContext<CatalogoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(mt =>
{
    mt.AddConsumer<PedidoCriadoConsumer>();
    mt.AddConsumer<PedidoConfirmadoConsumer>();
    mt.AddConsumer<PedidoCanceladoConsumer>();

    mt.UsingRabbitMq((ctx, cfg) =>
    {
        var rmq = builder.Configuration.GetSection("RabbitMQ");
        cfg.Host(rmq["Host"], rmq["VirtualHost"], h =>
        {
            h.Username(rmq["Username"]!);
            h.Password(rmq["Password"]!);
        });

        cfg.Message<PedidoCriadoEvento>(m => m.SetEntityName("pedido-criado-evento"));
        cfg.Message<ProdutosReservadosEvento>(m => m.SetEntityName("produtos-reservados-evento"));
        cfg.Message<PedidoConfirmadoEvento>(m => m.SetEntityName("pedido-confirmado-evento"));
        cfg.Message<PedidoCanceladoEvento>(m => m.SetEntityName("pedido-cancelado-evento"));

        cfg.ReceiveEndpoint("catalogo-pedido-criado", e =>
        {
            e.ConfigureConsumer<PedidoCriadoConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("catalogo-pedido-confirmado", e =>
        {
            e.ConfigureConsumer<PedidoConfirmadoConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("catalogo-pedido-cancelado", e =>
        {
            e.ConfigureConsumer<PedidoCanceladoConsumer>(ctx);
        });

        cfg.ConfigureEndpoints(ctx);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();