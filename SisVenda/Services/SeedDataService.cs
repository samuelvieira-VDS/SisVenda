using Sisvenda.Database;
using Sisvenda.Models;

namespace Sisvenda.Services
{
    /// <summary>
    /// Serviço para popular o banco de dados com dados de exemplo (útil para testes)
    /// </summary>
    public class SeedDataService
    {
        private readonly DatabaseService _databaseService;

        public SeedDataService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Popula o banco de dados com produtos de exemplo
        /// </summary>
        public async Task SeedDadosExemploAsync()
        {
            try
            {
                var produtos = new List<Produto>
                {
                    new Produto
                    {
                        Codigo = "P001",
                        Nome = "Notebook Dell Inspiron 15",
                        Descricao = "Notebook 15.6\" Full HD, Intel i5, 8GB RAM, 256GB SSD",
                        Preco = 3500.00m,
                        PrecoCusto = 2800.00m
                    },
                    new Produto
                    {
                        Codigo = "P002",
                        Nome = "Mouse Logitech MX Master 3",
                        Descricao = "Mouse sem fio, 4K resolution, 1-year battery",
                        Preco = 450.00m,
                        PrecoCusto = 300.00m
                    },
                    new Produto
                    {
                        Codigo = "P003",
                        Nome = "Teclado Mecânico RGB",
                        Descricao = "Teclado mecânico com switches Cherry MX, RGB backlit",
                        Preco = 580.00m,
                        PrecoCusto = 380.00m
                    },
                    new Produto
                    {
                        Codigo = "P004",
                        Nome = "Monitor LG 27 4K",
                        Descricao = "Monitor 27\" UltraHD 4K, 60Hz, IPS Panel",
                        Preco = 1800.00m,
                        PrecoCusto = 1400.00m
                    },
                    new Produto
                    {
                        Codigo = "P005",
                        Nome = "Webcam Logitech C922",
                        Descricao = "Webcam 1080p com foco automático e microfone integrado",
                        Preco = 380.00m,
                        PrecoCusto = 220.00m
                    },
                    new Produto
                    {
                        Codigo = "P006",
                        Nome = "Headset Corsair Void Elite",
                        Descricao = "Headset sem fio para gaming, Dolby 7.1 Surround",
                        Preco = 650.00m,
                        PrecoCusto = 420.00m
                    },
                    new Produto
                    {
                        Codigo = "P007",
                        Nome = "SSD Samsung 860 EVO 500GB",
                        Descricao = "SSD 2.5\", SATA III, 550MB/s read speed",
                        Preco = 350.00m,
                        PrecoCusto = 180.00m
                    },
                    new Produto
                    {
                        Codigo = "P008",
                        Nome = "RAM Corsair Vengeance 16GB",
                        Descricao = "Memória RAM DDR4, 3200MHz, CAS Latency 16",
                        Preco = 420.00m,
                        PrecoCusto = 250.00m
                    }
                };

                foreach (var produto in produtos)
                {
                    await _databaseService.AddProdutoAsync(produto);
                }

                System.Diagnostics.Debug.WriteLine("✅ Dados de exemplo inseridos com sucesso!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao inserir dados de exemplo: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se o banco de dados está vazio e popula com dados de exemplo se necessário
        /// </summary>
        public async Task SeedIfEmptyAsync()
        {
            try
            {
                var produtos = await _databaseService.GetProdutosAsync();
                if (produtos.Count == 0)
                {
                    await SeedDadosExemploAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Erro ao verificar banco de dados: {ex.Message}");
            }
        }
    }
}
