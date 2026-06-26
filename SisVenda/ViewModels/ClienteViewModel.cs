using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sisvenda.Database;
using Sisvenda.Models;
using Sisvenda.Models;
using Sisvenda.Views;

namespace Sisvenda.ViewModels
{
    public class ClienteViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Cliente> _clientes;
        private Cliente _clienteSelecionado;
        private bool _isLoading;

        public ObservableCollection<Cliente> Clientes
        {
            get => _clientes;
            set
            {
                if (_clientes != value)
                {
                    _clientes = value;
                    OnPropertyChanged();
                }
            }
        }

        public Cliente ClienteSelecionado
        {
            get => _clienteSelecionado;
            set
            {
                if (_clienteSelecionado != value)
                {
                    _clienteSelecionado = value;
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

        public ICommand LoadClientesCommand { get; }
        public ICommand AddClienteCommand { get; }
        public ICommand UpdateClienteCommand { get; }
        public ICommand DeleteClienteCommand { get; }

        public ClienteViewModel()
        {
            _databaseService = new DatabaseService();
            _clientes = new ObservableCollection<Cliente>();

            LoadClientesCommand = new Command(async () => await LoadClientesAsync());
            AddClienteCommand = new Command<Cliente>(async (c) => await AddClienteAsync(c));
            UpdateClienteCommand = new Command<Cliente>(async (c) => await UpdateClienteAsync(c));
            DeleteClienteCommand = new Command<Cliente>(async (c) => await DeleteClienteAsync(c));
        }

        public async Task LoadClientesAsync()
        {
            try
            {
                IsLoading = true;
                var clientes = await _databaseService.GetClientesAsync();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Clientes.Clear();
                    foreach (var cliente in clientes)
                    {
                        Clientes.Add(cliente);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao carregar clientes: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task AddClienteAsync(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                    return;

                IsLoading = true;
                await _databaseService.AddClienteAsync(cliente);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Clientes.Add(cliente);
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Cliente adicionado com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao adicionar cliente: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UpdateClienteAsync(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                    return;

                IsLoading = true;
                await _databaseService.UpdateClienteAsync(cliente);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var index = Clientes.IndexOf(cliente);
                    if (index >= 0)
                    {
                        Clientes[index] = cliente;
                    }
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Cliente atualizado com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao atualizar cliente: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task DeleteClienteAsync(Cliente cliente)
        {
            try
            {
                if (cliente == null)
                    return;

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirmação",
                    $"Deseja realmente deletar o cliente '{cliente.Nome}'?",
                    "Sim", "Não");

                if (!confirm)
                    return;

                IsLoading = true;
                await _databaseService.DeleteClienteAsync(cliente);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Clientes.Remove(cliente);
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Cliente deletado com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao deletar cliente: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<List<Cliente>> GetClientesByNomeAsync(string nome)
        {
            try
            {
                IsLoading = true;
                return await _databaseService.SearchClientesByNome(nome);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao buscar clientes: {ex.Message}", "OK");
                return new List<Cliente>();
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
