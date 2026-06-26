using Sisvenda.Models;
using Sisvenda.ViewModels;

namespace Sisvenda.Views
{
    public partial class VendaListPage : ContentPage
    {
        private VendaViewModel _viewModel;

        public VendaListPage()
        {
            InitializeComponent();
            _viewModel = new VendaViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadVendasAsync();
        }

        private async void OnAddVendaClicked(object sender, EventArgs e)
        {
            var page = new VendaDetailPage();
            await Navigation.PushAsync(page);
        }

        private async void OnEditVendaClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Venda venda)
            {
                var page = new VendaDetailPage { BindingContext = venda };
                await Navigation.PushAsync(page);
            }
        }

        private async void OnDeleteVendaClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Venda venda)
            {
                await _viewModel.DeleteVendaAsync(venda);
            }
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                await _viewModel.LoadVendasAsync();
                return;
            }

            // Tentativa de buscar por nome de cliente
            var vendas = await _viewModel.GetVendasByClienteAsync(-1);
            _viewModel.Vendas.Clear();
            foreach (var venda in vendas)
            {
                _viewModel.Vendas.Add(venda);
            }
        }
    }
}
