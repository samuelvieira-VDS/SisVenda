using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sisvenda.Database;
using Sisvenda.Models;

namespace Sisvenda.ViewModels
{
    public class ProdutoViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Produto> _produtos;
        private Produto _produtoSelecionado;
        private bool _isLoading;

        public ObservableCollection<Produto> Produtos
        {
            get => _produtos;
            set
            {
                if (_produtos != value)
                {
                    _produtos = value;
                    OnPropertyChanged();
                }
            }
        }

        public Produto ProdutoSelecionado
        {
            get => _produtoSelecionado;
            set
            {
                if (_produtoSelecionado != value)
                {
                    _produtoSelecionado = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoadProdutosCommand { get; }
        public ICommand AddProdutoCommand { get; }
        public ICommand UpdateProdutoCommand { get; }
        public ICommand DeleteProdutoCommand { get; }

        public ProdutoViewModel()
        {
            _databaseService = new DatabaseService();
            _produtos = new ObservableCollection<Produto>();

            LoadProdutosCommand = new Command(async () => await LoadProdutosAsync());
            AddProdutoCommand = new Command<Produto>(async (p) => await AddProdutoAsync(p));
            UpdateProdutoCommand = new Command<Produto>(async (p) => await UpdateProdutoAsync(p));
            DeleteProdutoCommand = new Command<Produto>(async (p) => await DeleteProdutoAsync(p));
        }

        public async Task LoadProdutosAsync()
        {
            try
            {
                IsLoading = true;
                var produtos = await _databaseService.GetProdutosAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Produtos.Clear();
                    foreach (var produto in produtos)
                    {
                        Produtos.Add(produto);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao carregar produtos: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task AddProdutoAsync(Produto produto)
        {
            try
            {
                if (produto == null)
                    return;

                IsLoading = true;
                await _databaseService.AddProdutoAsync(produto);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Produtos.Add(produto);
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Produto adicionado com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao adicionar produto: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UpdateProdutoAsync(Produto produto)
        {
            try
            {
                if (produto == null)
                    return;

                IsLoading = true;
                await _databaseService.UpdateProdutoAsync(produto);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var index = Produtos.IndexOf(produto);
                    if (index >= 0)
                    {
                        Produtos[index] = produto;
                    }
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Produto atualizado com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao atualizar produto: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task DeleteProdutoAsync(Produto produto)
        {
            try
            {
                if (produto == null)
                    return;

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirmação",
                    $"Deseja realmente deletar o produto '{produto.Nome}'?",
                    "Sim", "Não");

                if (!confirm)
                    return;

                IsLoading = true;
                await _databaseService.DeleteProdutoAsync(produto);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Produtos.Remove(produto);
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Produto deletado com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao deletar produto: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<List<Produto>> GetProdutosByNomeAsync(string nome)
        {
            try
            {
                IsLoading = true;
                return await _databaseService.SearchProdutosByNome(nome);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao buscar produtos: {ex.Message}", "OK");
                return new List<Produto>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
