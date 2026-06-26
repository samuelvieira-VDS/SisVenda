using SQLite;
using System.Collections.Generic;

namespace Sisvenda.Models
{
    [Table("Vendas")]
    public class Venda
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int IdCliente { get; set; }

        public DateTime Data { get; set; }

        public decimal ValorTotal { get; set; }

        [Ignore]
        public List<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
    }
}
