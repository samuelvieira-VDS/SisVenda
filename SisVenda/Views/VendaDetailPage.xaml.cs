using Sisvenda.Models;
using Sisvenda.ViewModels;
using System.Collections.ObjectModel;

namespace Sisvenda.Views
{
    public partial class VendaDetailPage : ContentPage
    {
        private Venda _venda;
        private List<Produto> _produtos;
        private List<Cliente> _clientes;
        private ObservableCollection<ItemVenda> _itens;

        public ObservableCollection<ItemVenda> Itens
        {
            get => _itens;
            set => _itens = value;
        }

        public decimal ValorTotal => _venda?.ValorTotal ?? 0m;

        public DateTime CurrentDate
        {
            get => _venda?.Data ?? DateTime.Now;
            set
            {
                if (_venda != null)
                {
                    _venda.Data = value;
                }
            }
        }

        public Cliente ClienteSelecionado { get; set; }

        public VendaDetailPage()
        {
            InitializeComponent();
            _itens = new ObservableCollection<ItemVenda>();
            _venda = new Venda
            {
                Data = DateTime.Now,
                ValorTotal = 0m,
                Itens = new List<ItemVenda>()
            };

            BindingContext = this;

            // Inicializar pickers
            LoadClientesAsync();
            LoadProdutosAsync();
        }

        private async void LoadClientesAsync()
        {
            var db = new Database.DatabaseService();
            _clientes = await db.GetClientesAsync();
            ClientePicker.ItemsSource = _clientes;
            ClientePicker.ItemDisplayBinding = new Binding("Nome");
        }

        private async void LoadProdutosAsync()
        {
            var db = new Database.DatabaseService();
            _produtos = await db.GetProdutosAsync();
            ProdutoPicker.ItemsSource = _produtos;
            ProdutoPicker.ItemDisplayBinding = new Binding("Nome");
        }

        private void OnClienteSelectedChanged(object sender, EventArgs e)
        {
            if (ClientePicker.SelectedItem is Cliente cliente)
            {
                _venda.IdCliente = cliente.Id;
                ClienteSelecionado = cliente;
            }
        }

        private void OnProdutoSelectedChanged(object sender, EventArgs e)
        {
            // nothing for now
        }

        private async void OnAdicionarItemClicked(object sender, EventArgs e)
        {
            if (ProdutoPicker.SelectedItem is Produto produto)
            {
                if (!decimal.TryParse(QuantidadeEntry.Text, out var quantidade) || quantidade <= 0)
                {
                    await DisplayAlert("Validação", "Informe uma quantidade válida", "OK");
                    return;
                }

                var item = new ItemVenda
                {
                    IdProduto = produto.Id,
                    Quantidade = quantidade,
                    Valor = produto.Preco,
                    ValorTotal = quantidade * produto.Preco
                };

                _venda.Itens.Add(item);
                _itens.Add(item);

                // Limpar campos
                QuantidadeEntry.Text = "";
                ProdutoPicker.SelectedItem = null;

                RecalcularTotais();
            }
            else
            {
                await DisplayAlert("Validação", "Selecione um produto", "OK");
            }
        }

        private async void OnRemoverItemClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is ItemVenda item)
            {
                _venda.Itens.Remove(item);
                _itens.Remove(item);
                RecalcularTotais();
            }
        }

        private void RecalcularTotais()
        {
            _venda.ValorTotal = _venda.Itens.Sum(i => i.ValorTotal);
            // Notificar UI sobre mudança de ValorTotal
            OnPropertyChanged(nameof(ValorTotal));
        }

        private async void OnSaveVendaClicked(object sender, EventArgs e)
        {
            if (_venda.IdCliente <= 0)
            {
                await DisplayAlert("Validação", "Selecione um cliente", "OK");
                return;
            }

            if (_venda.Itens == null || !_venda.Itens.Any())
            {
                await DisplayAlert("Validação", "Adicione pelo menos um produto à venda", "OK");
                return;
            }

            try
            {
                var db = new Database.DatabaseService();
                // Salva venda
                await db.AddVendaAsync(_venda);

                // Salva itens com IdVenda preenchido
                foreach (var item in _venda.Itens)
                {
                    item.IdVenda = _venda.Id;
                    await db.AddItemVendaAsync(item);
                }

                await DisplayAlert("Sucesso", "Venda salva com sucesso!", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao salvar venda: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
