using SQLite;

namespace Sisvenda.Models
{
    [Table("Produtos")]
    public class Produto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        [MaxLength(50)]
        public string Codigo { get; set; }

        [MaxLength(200)]
        public string Nome { get; set; }

        [MaxLength(500)]
        public string Descricao { get; set; }

        public decimal Preco { get; set; }

        public decimal PrecoCusto { get; set; }
    }
}
