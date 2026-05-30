using Sisvenda.Models;
using Sisvenda.ViewModels;

namespace Sisvenda.Views
{
    [QueryProperty(nameof(ProductId), "id")]
    public partial class ProdutoDetailPage : ContentPage
    {
        private ProdutoDetailViewModel _viewModel;
        private int _productId = -1;
        private Produto _produto;

        public int ProductId
        {
            get => _productId;
            set => _productId = value;
        }

        public ProdutoDetailPage()
        {
            InitializeComponent();
            _viewModel = new ProdutoDetailViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Se o BindingContext é um Produto (vem da edição)
            if (BindingContext is Produto produto && produto != null && produto.Id > 0)
            {
                // Cria um novo ViewModel para a edição
                _viewModel = new ProdutoDetailViewModel();
                _viewModel.Produto = produto;
                _viewModel.TituloPagina = "Editar Produto";
                BindingContext = _viewModel;
            }
            else
            {
                // Senão, inicializa como novo
                await _viewModel.InitializeAsync(_productId > 0 ? _productId : null);
            }
        }

        private async void OnSalvarClicked(object sender, EventArgs e)
        {
            if (await _viewModel.SalvarProdutoAsync())
            {
                await Navigation.PopAsync();
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
