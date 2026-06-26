using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Sisvenda.Database;
using Sisvenda.Models;

namespace Sisvenda.ViewModels
{
    public class VendaViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Venda> _vendas;
        private Venda _vendaSelecionada;
        private bool _isLoading;

        public ObservableCollection<Venda> Vendas
        {
            get => _vendas;
            set
            {
                if (_vendas != value)
                {
                    _vendas = value;
                    OnPropertyChanged();
                }
            }
        }

        public Venda VendaSelecionada
        {
            get => _vendaSelecionada;
            set
            {
                if (_vendaSelecionada != value)
                {
                    _vendaSelecionada = value;
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

        public ICommand LoadVendasCommand { get; }
        public ICommand AddVendaCommand { get; }
        public ICommand UpdateVendaCommand { get; }
        public ICommand DeleteVendaCommand { get; }

        public VendaViewModel()
        {
            _databaseService = new DatabaseService();
            _vendas = new ObservableCollection<Venda>();

            LoadVendasCommand = new Command(async () => await LoadVendasAsync());
            AddVendaCommand = new Command<Venda>(async (v) => await AddVendaAsync(v));
            UpdateVendaCommand = new Command<Venda>(async (v) => await UpdateVendaAsync(v));
            DeleteVendaCommand = new Command<Venda>(async (v) => await DeleteVendaAsync(v));
        }

        public async Task LoadVendasAsync()
        {
            try
            {
                IsLoading = true;
                var vendas = await _databaseService.GetVendasAsync();

                // Carregar itens para cada venda
                foreach (var venda in vendas)
                {
                    venda.Itens = await _databaseService.GetItensPorVendaAsync(venda.Id);
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Vendas.Clear();
                    foreach (var venda in vendas)
                    {
                        Vendas.Add(venda);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao carregar vendas: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task AddVendaAsync(Venda venda)
        {
            try
            {
                if (venda == null)
                    return;

                IsLoading = true;
                await _databaseService.AddVendaAsync(venda);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Vendas.Add(venda);
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Venda adicionada com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao adicionar venda: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task UpdateVendaAsync(Venda venda)
        {
            try
            {
                if (venda == null)
                    return;

                IsLoading = true;
                await _databaseService.UpdateVendaAsync(venda);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var index = Vendas.IndexOf(venda);
                    if (index >= 0)
                    {
                        Vendas[index] = venda;
                    }
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Venda atualizada com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao atualizar venda: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task DeleteVendaAsync(Venda venda)
        {
            try
            {
                if (venda == null)
                    return;

                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Confirmação",
                    $"Deseja realmente deletar a venda de {venda.Data:dd/MM/yyyy}?",
                    "Sim", "Não");

                if (!confirm)
                    return;

                IsLoading = true;
                await _databaseService.DeleteVendaAsync(venda);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Vendas.Remove(venda);
                });

                await Application.Current.MainPage.DisplayAlert("Sucesso", "Venda deletada com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao deletar venda: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task<List<Venda>> GetVendasByClienteAsync(int idCliente)
        {
            try
            {
                IsLoading = true;
                return await _databaseService.SearchVendasByCliente(idCliente);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Erro ao buscar vendas: {ex.Message}", "OK");
                return new List<Venda>();
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
