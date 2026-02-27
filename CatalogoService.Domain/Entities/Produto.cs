using CatalogoService.Domain.Common;
using CatalogoService.Domain.Enums;

namespace CatalogoService.Domain.Entities
{
    public class Produto : BaseEntity
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; private set; }
        public decimal Preco { get; private set; }
        public string? ImagemUrl { get; private set; }
        public ProdutoStatus Status { get; private set; }

        public Guid CategoriaId { get; private set; }
        public Categoria Categoria { get; private set; } = null!;
        private Produto() { }

        public static Produto Create(
            string nome,
            decimal preco,
            Guid categoriaId,
            string? descricao = null,
            string? imagemUrl = null)
                {
                return new Produto
                {
                    Nome = nome,
                    Preco = preco,
                    Descricao = descricao,
                    CategoriaId = categoriaId,
                    ImagemUrl = imagemUrl,
                    Status = ProdutoStatus.Disponivel
                };
            }

            public void Update(
                string nome,
                decimal preco,
                Guid categoriaId,
                string? descricao = null,
                string? imagemUrl = null)
            {
                Nome = nome;
                Preco = preco;
                Descricao = descricao;
                CategoriaId = categoriaId;
                ImagemUrl = imagemUrl;
            }

        public void Reservar()
        {
            Status = ProdutoStatus.Reservado;
            SetAtualizadoEm();
        }
        public void Indisponibilizar()
        {
            Status = ProdutoStatus.Indisponivel;
            SetAtualizadoEm();
        }

    }
}
