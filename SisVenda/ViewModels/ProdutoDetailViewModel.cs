using System.ComponentModel;
using System.Runtime.CompilerServices;
using Sisvenda.Database;
using Sisvenda.Models;

namespace Sisvenda.ViewModels
{
    public class ProdutoDetailViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private Produto _produto;
        private string _tituloPagina;
        private bool _isEdicao;

        public Produto Produto
        {
            get => _produto;
            set
            {
                if (_produto != value)
                {
                    _produto = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MargemLucro));
                }
            }
        }

        public string TituloPagina
        {
            get => _tituloPagina;
            set
            {
                if (_tituloPagina != value)
                {
                    _tituloPagina = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal MargemLucro
        {
            get
            {
                if (_produto?.PrecoCusto == 0)
                    return 0;

                return ((_produto.Preco - _produto.PrecoCusto) / _produto.PrecoCusto) * 100;
            }
        }

        public ProdutoDetailViewModel()
        {
            _databaseService = new DatabaseService();
            _produto = new Produto();
            _tituloPagina = "Novo Produto";
            _isEdicao = false;
        }

        public async Task InitializeAsync(int? produtoId)
        {
            if (produtoId.HasValue && produtoId.Value > 0)
            {
                _isEdicao = true;
                Produto = await _databaseService.GetProdutoAsync(produtoId.Value);
                TituloPagina = "Editar Produto";
            }
            else
            {
                _isEdicao = false;
                Produto = new Produto();
                TituloPagina = "Novo Produto";
            }
        }

        public async Task<bool> SalvarProdutoAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_produto.Codigo))
                {
                    await Application.Current.MainPage.DisplayAlert("Validação", "O código do produto é obrigatório!", "OK");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(_produto.Nome))
                {
                    await Application.Current.MainPage.DisplayAlert("Validação", "O nome do produto é obrigatório!", "OK");
                    return false;
                }

                if (_produto.Preco < 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Validação", "O preço deve ser maior ou igual a zero!", "OK");
                    return false;
                }

                if (_produto.PrecoCusto < 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Validação", "O preço de custo deve ser maior ou igual a zero!", "OK");
                    return false;
                }

                if (_isEdicao)
                {
                    await _databaseService.UpdateProdutoAsync(_produto);
                    await Application.Current.MainPage.DisplayAlert("Sucesso", "Produto atualizado com sucesso!", "OK");
                }
                else
                {
                    await _databaseService.AddProdutoAsync(_produto);
                    await Application.Current.MainPage.DisplayAlert("Sucesso", "Produto adicionado com sucesso!", "OK");
                }

                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao salvar produto: {ex.Message}", "OK");
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
