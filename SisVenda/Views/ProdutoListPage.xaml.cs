using Sisvenda.Models;
using Sisvenda.ViewModels;

namespace Sisvenda.Views
{
    public partial class ProdutoListPage : ContentPage
    {
        private ProdutoViewModel _viewModel;

        public ProdutoListPage()
        {
            InitializeComponent();
            _viewModel = new ProdutoViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadProdutosAsync();
        }

        private async void OnAddProdutoClicked(object sender, EventArgs e)
        {
            var page = new ProdutoDetailPage();
            await Navigation.PushAsync(page);
        }

        private async void OnEditProdutoClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Produto produto)
            {
                var page = new ProdutoDetailPage { BindingContext = produto };
                await Navigation.PushAsync(page);
            }
        }

        private async void OnDeleteProdutoClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Produto produto)
            {
                await _viewModel.DeleteProdutoAsync(produto);
            }
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                await _viewModel.LoadProdutosAsync();
                return;
            }

            var produtos = await _viewModel.GetProdutosByNomeAsync(e.NewTextValue);
            _viewModel.Produtos.Clear();
            foreach (var produto in produtos)
            {
                _viewModel.Produtos.Add(produto);
            }
        }
    }
}
