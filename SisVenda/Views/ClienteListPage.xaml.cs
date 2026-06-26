using Sisvenda.Models;
using Sisvenda.ViewModels;

namespace Sisvenda.Views
{
    public partial class ClienteListPage : ContentPage
    {
        private readonly ClienteViewModel _viewModel;

        public ClienteListPage()
        {
            InitializeComponent();
            _viewModel = new ClienteViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadClientesAsync();
        }

        private async void OnAddClienteClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///clienteDetail");
        }

        // NOTE: Other GoToAsync calls may also need absolute routes ("///") if they target Shell elements.


        private async void OnEditClienteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Cliente cliente)
            {
                await Shell.Current.GoToAsync($"///clienteDetail?clienteId={cliente.Id}");
            }
        }

        private async void OnDeleteClienteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Cliente cliente)
            {
                await _viewModel.DeleteClienteAsync(cliente);
            }
        }

        private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var text = e.NewTextValue;
            if (string.IsNullOrWhiteSpace(text))
            {
                await _viewModel.LoadClientesAsync();
                return;
            }

            var byCpf = await _viewModel.GetClientesByNomeAsync(text);
            // Também busca por CPF/CNPJ
            var byName = await _viewModel.GetClientesByNomeAsync(text);

            // Merge results (simple approach)
            var combined = byCpf.Union(byName).ToList();

            _viewModel.Clientes.Clear();
            foreach (var c in combined)
                _viewModel.Clientes.Add(c);
        }
    }
}
