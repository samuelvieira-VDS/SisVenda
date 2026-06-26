using Sisvenda.Models;
using Sisvenda.ViewModels;

namespace Sisvenda.Views
{
    [QueryProperty(nameof(ClienteId), "clienteId")]
    public partial class ClienteDetailPage : ContentPage
    {
        private readonly ClienteViewModel _viewModel;
        private int _clienteId = -1;
        private Cliente _cliente;

        public int ClienteId
        {
            get => _clienteId;
            set
            {
                _clienteId = value;
                LoadClienteDetails(_clienteId);
            }
        }

        public ClienteDetailPage()
        {
            InitializeComponent();
            _viewModel = new ClienteViewModel();
            BindingContext = _viewModel;
        }

        private async void LoadClienteDetails(int clienteId)
        {
            if (clienteId <= 0)
            {
                Title = "Novo Cliente";
                return;
            }

            try
            {
                var dbService = new Database.DatabaseService();
                _cliente = await dbService.GetClienteAsync(clienteId);

                if (_cliente != null)
                {
                    Title = "Editar Cliente";
                    PopulateFields(_cliente);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao carregar cliente: {ex.Message}", "OK");
            }
        }

        private void PopulateFields(Cliente cliente)
        {
            CpfCnpjEntry.Text = cliente.CpfCnpj;
            NomeEntry.Text = cliente.Nome;
            EnderecoEntry.Text = cliente.Endereco;
            NumeroEntry.Text = cliente.Numero;
            ComplementoEntry.Text = cliente.Complemento;
            TelefoneEntry.Text = cliente.Telefone;
            BairroEntry.Text = cliente.Bairro;
            CidadeEntry.Text = cliente.Cidade;
            CepEntry.Text = cliente.Cep;
        }

        private async void OnSaveClienteClicked(object sender, EventArgs e)
        {
            if (!ValidateFields())
            {
                await DisplayAlert("Validação", "Preencha todos os campos obrigatórios", "OK");
                return;
            }

            try
            {
                if (_cliente == null)
                {
                    // Novo cliente
                    _cliente = new Cliente
                    {
                        CpfCnpj = CpfCnpjEntry.Text,
                        Nome = NomeEntry.Text,
                        Endereco = EnderecoEntry.Text,
                        Numero = NumeroEntry.Text,
                        Complemento = ComplementoEntry.Text,
                        Telefone = TelefoneEntry.Text,
                        Bairro = BairroEntry.Text,
                        Cidade = CidadeEntry.Text,
                        Cep = CepEntry.Text
                    };

                    await _viewModel.AddClienteAsync(_cliente);
                }
                else
                {
                    // Atualizando cliente existente
                    _cliente.CpfCnpj = CpfCnpjEntry.Text;
                    _cliente.Nome = NomeEntry.Text;
                    _cliente.Endereco = EnderecoEntry.Text;
                    _cliente.Numero = NumeroEntry.Text;
                    _cliente.Complemento = ComplementoEntry.Text;
                    _cliente.Telefone = TelefoneEntry.Text;
                    _cliente.Bairro = BairroEntry.Text;
                    _cliente.Cidade = CidadeEntry.Text;
                    _cliente.Cep = CepEntry.Text;

                    await _viewModel.UpdateClienteAsync(_cliente);
                }

                await Shell.Current.GoToAsync("/clientes", true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao salvar cliente: {ex.Message}", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("///clientes", true);
        }

        private bool ValidateFields()
        {
            return !string.IsNullOrWhiteSpace(CpfCnpjEntry.Text) &&
                   !string.IsNullOrWhiteSpace(NomeEntry.Text) &&
                   !string.IsNullOrWhiteSpace(EnderecoEntry.Text) &&
                   !string.IsNullOrWhiteSpace(NumeroEntry.Text) &&
                   !string.IsNullOrWhiteSpace(TelefoneEntry.Text) &&
                   !string.IsNullOrWhiteSpace(BairroEntry.Text) &&
                   !string.IsNullOrWhiteSpace(CidadeEntry.Text) &&
                   !string.IsNullOrWhiteSpace(CepEntry.Text);
        }
    }
}
