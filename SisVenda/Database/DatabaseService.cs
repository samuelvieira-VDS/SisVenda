using SQLite;
using Sisvenda.Models;

namespace Sisvenda.Database
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _db;
        private const string DbFileName = "sisvenda.db";
        private string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, DbFileName);
        }

        /// <summary>
        /// Inicializa a conexão com o banco de dados e cria as tabelas
        /// </summary>
        public async Task InitializeAsync()
        {
            if (_db != null)
                return;

            _db = new SQLiteAsyncConnection(_dbPath);
            await _db.CreateTableAsync<Produto>();
        }

        /// <summary>
        /// Adiciona um novo produto ao banco de dados
        /// </summary>
        public async Task<int> AddProdutoAsync(Produto produto)
        {
            await InitializeAsync();
            return await _db.InsertAsync(produto);
        }

        /// <summary>
        /// Obtém todos os produtos
        /// </summary>
        public async Task<List<Produto>> GetProdutosAsync()
        {
            await InitializeAsync();
            return await _db.Table<Produto>().ToListAsync();
        }

        /// <summary>
        /// Obtém um produto pelo ID
        /// </summary>
        public async Task<Produto> GetProdutoAsync(int id)
        {
            await InitializeAsync();
            return await _db.Table<Produto>()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Atualiza um produto existente
        /// </summary>
        public async Task<int> UpdateProdutoAsync(Produto produto)
        {
            await InitializeAsync();
            return await _db.UpdateAsync(produto);
        }

        /// <summary>
        /// Deleta um produto pelo ID
        /// </summary>
        public async Task<int> DeleteProdutoAsync(int id)
        {
            await InitializeAsync();
            return await _db.DeleteAsync<Produto>(id);
        }

        /// <summary>
        /// Deleta um produto
        /// </summary>
        public async Task<int> DeleteProdutoAsync(Produto produto)
        {
            await InitializeAsync();
            return await _db.DeleteAsync(produto);
        }

        /// <summary>
        /// Busca produtos por código
        /// </summary>
        public async Task<List<Produto>> SearchProdutosByCodigo(string codigo)
        {
            await InitializeAsync();
            return await _db.Table<Produto>()
                .Where(p => p.Codigo.Contains(codigo))
                .ToListAsync();
        }

        /// <summary>
        /// Busca produtos por nome
        /// </summary>
        public async Task<List<Produto>> SearchProdutosByNome(string nome)
        {
            await InitializeAsync();
            return await _db.Table<Produto>()
                .Where(p => p.Nome.Contains(nome))
                .ToListAsync();
        }

        /// <summary>
        /// Limpa o banco de dados (útil para testes)
        /// </summary>
        public async Task ClearAsync()
        {
            await InitializeAsync();
            await _db.DeleteAllAsync<Produto>();
        }
    }
}
