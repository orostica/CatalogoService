using CatalogoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogoService.Infrastructure.Persistence.Configurations;
public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("produto");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.Nome)
            .HasColumnName("nome")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.Preco)
            .HasColumnName("preco")
            .IsRequired();

        builder.Property(c => c.ImagemUrl)
            .HasColumnName("imagem_url")
            .HasMaxLength(2000);

        builder.Property(c => c.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(c => c.CriadoEm)
            .HasColumnName("criado_em")
            .IsRequired();

        builder.Property(c => c.AtualizadoEm)
            .HasColumnName("atualizado_em")
            .IsRequired();

        builder.Property(c => c.CategoriaId)
            .HasColumnName("categoria_id")
            .IsRequired();

        builder.HasIndex(c => c.Nome);
    }
}
