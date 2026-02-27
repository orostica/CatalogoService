using CatalogoService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace CatalogoService.Domain.Entities
{
    public class Categoria : BaseEntity
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao {  get; set; }

        private Categoria() { }

        public static Categoria Create(string nome, string? descricao = null)
        {
            return new Categoria
            {
                Nome = nome,
                Descricao = descricao,
            };
        }

        public void Update(string name, string? description = null)
        {
            Nome = name;
            Descricao = description;
            SetAtualizadoEm();
        }
    }
}
