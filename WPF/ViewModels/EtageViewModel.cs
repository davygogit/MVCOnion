using System.Collections.ObjectModel;
using System.Windows.Input;
using Domain;
using WPF.Commands;
using WPF.Services;

namespace WPF.ViewModels
{
    public class EtageViewModel : BaseViewModel
    {
        private readonly IRepository<Etage> _etageRepository;
        private readonly IDialogService _dialogService;
        private readonly IImageService _imageService;

        private ObservableCollection<Etage> _etages = new();
        private Etage? _selectedEtage;
        private bool _isLoading;

        public ObservableCollection<Etage> Etages
        {
            get => _etages;
            set => SetProperty(ref _etages, value);
        }

        public Etage? SelectedEtage
        {
            get => _selectedEtage;
            set => SetProperty(ref _selectedEtage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadDataCommand { get; }

        public EtageViewModel(
            IRepository<Etage> etageRepository,
            IDialogService dialogService,
            IImageService imageService)
        {
            _etageRepository = etageRepository;
            _dialogService = dialogService;
            _imageService = imageService;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                // Utilisation de GetAllAsync() au lieu de GetAll().ToList()
                var etages = await _etageRepository.GetAllAsync();
                Etages = new ObservableCollection<Etage>(etages);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError("Erreur", $"Erreur lors du chargement: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
