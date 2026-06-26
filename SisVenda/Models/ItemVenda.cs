using SQLite;

namespace Sisvenda.Models
{
    [Table("ItemVendas")]
    public class ItemVenda
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Foreign key to Venda
        public int IdVenda { get; set; }

        public int IdProduto { get; set; }

        public decimal Quantidade { get; set; }

        public decimal Valor { get; set; }

        public decimal ValorTotal { get; set; }
    }
}
