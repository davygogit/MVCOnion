using System.Collections.ObjectModel;
using System.Windows.Input;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using WPF.Commands;
using WPF.Services;

namespace WPF.ViewModels
{
    public class SalleListViewModel : BaseViewModel
    {
        private readonly IRepository<Salle> _salleRepository;
        private readonly IRepository<Etage> _etageRepository;
        private readonly ISalleManager _salleManager;
        private readonly IDialogService _dialogService;

        private ObservableCollection<Salle> _salles = new();
        private ObservableCollection<Salle> _filteredSalles = new();
        private ObservableCollection<Etage> _etages = new();
        private Salle? _selectedSalle;
        private Etage? _selectedEtage;
        private string _searchText = string.Empty;
        private TypeSalle? _selectedTypeSalle;
        private bool _isLoading;

        public ObservableCollection<Salle> Salles
        {
            get => _salles;
            set => SetProperty(ref _salles, value);
        }

        public ObservableCollection<Salle> FilteredSalles
        {
            get => _filteredSalles;
            set => SetProperty(ref _filteredSalles, value);
        }

        public ObservableCollection<Etage> Etages
        {
            get => _etages;
            set => SetProperty(ref _etages, value);
        }

        public Salle? SelectedSalle
        {
            get => _selectedSalle;
            set => SetProperty(ref _selectedSalle, value);
        }

        public Etage? SelectedEtage
        {
            get => _selectedEtage;
            set
            {
                if (SetProperty(ref _selectedEtage, value))
                    ApplyFilters();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplyFilters();
            }
        }

        public TypeSalle? SelectedTypeSalle
        {
            get => _selectedTypeSalle;
            set
            {
                if (SetProperty(ref _selectedTypeSalle, value))
                    ApplyFilters();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoadDataCommand { get; }
        public ICommand ToggleFavoriCommand { get; }
        public ICommand DeleteSalleCommand { get; }
        public ICommand ResetFiltersCommand { get; }
        public ICommand ShowDetailsCommand { get; }

        public SalleListViewModel(
            IRepository<Salle> salleRepository,
            IRepository<Etage> etageRepository,
            ISalleManager salleManager,
            IDialogService dialogService)
        {
            _salleRepository = salleRepository;
            _etageRepository = etageRepository;
            _salleManager = salleManager;
            _dialogService = dialogService;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            ToggleFavoriCommand = new AsyncRelayCommand<Salle>(ToggleFavoriAsync);
            DeleteSalleCommand = new AsyncRelayCommand<Salle>(DeleteSalleAsync);
            ResetFiltersCommand = new RelayCommand(ResetFilters);
            ShowDetailsCommand = new RelayCommand<Salle>(ShowDetails);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            IsLoading = true;
            try
            {
                // Utilisation de GetQueryable() pour ajouter Include
                var salles = await _salleRepository.GetQueryable()
                    .Include(s => s.Etage)
                    .ToListAsync();
                
                Salles = new ObservableCollection<Salle>(salles);

                // Utilisation de GetQueryable() pour ajouter OrderBy
                var etages = await _etageRepository.GetQueryable()
                    .OrderBy(e => e.Niveau)
                    .ToListAsync();
                
                Etages = new ObservableCollection<Etage>(etages);

                ApplyFilters();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError("Erreur", $"Erreur lors du chargement des données: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilters()
        {
            var filtered = Salles.AsEnumerable();

            // Filtre par texte de recherche
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(s =>
                    (s.Nom?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    s.Numero.ToString().Contains(SearchText));
            }

            // Filtre par étage
            if (SelectedEtage != null)
            {
                filtered = filtered.Where(s => s.EtageId == SelectedEtage.Id);
            }

            // Filtre par type de salle
            if (SelectedTypeSalle.HasValue)
            {
                filtered = filtered.Where(s => s.TypeSalle == SelectedTypeSalle.Value);
            }

            FilteredSalles = new ObservableCollection<Salle>(filtered);
        }

        private async Task ToggleFavoriAsync(Salle? salle)
        {
            if (salle == null) return;

            try
            {
                salle.Favori = !salle.Favori;
                await _salleRepository.UpdateAsync(salle);
                await _salleRepository.SaveChangesAsync();
                
                _dialogService.ShowInformation("Succès", 
                    salle.Favori == true ? "Salle ajoutée aux favoris" : "Salle retirée des favoris");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError("Erreur", $"Erreur lors de la mise à jour: {ex.Message}");
            }
        }

        private async Task DeleteSalleAsync(Salle? salle)
        {
            if (salle == null) return;

            var result = _dialogService.ShowConfirmation("Confirmation", 
                $"Voulez-vous vraiment supprimer la salle {salle.Nom} ?");

            if (result)
            {
                try
                {
                    await _salleRepository.DeleteAsync(salle.Id); // Soft delete par défaut
                    await _salleRepository.SaveChangesAsync();
                    await LoadDataAsync();
                    
                    _dialogService.ShowInformation("Succès", "Salle supprimée avec succès");
                }
                catch (Exception ex)
                {
                    _dialogService.ShowError("Erreur", $"Erreur lors de la suppression: {ex.Message}");
                }
            }
        }

        private void ResetFilters()
        {
            SearchText = string.Empty;
            SelectedEtage = null;
            SelectedTypeSalle = null;
        }

        private void ShowDetails(Salle? salle)
        {
            if (salle == null) return;

            var details = $"Salle: {salle.Nom}\n" +
                         $"Numéro: {salle.Numero}\n" +
                         $"Type: {salle.TypeSalle}\n" +
                         $"Étage: {salle.Etage?.Nom}\n" +
                         $"Places: {salle.NbPlaces}\n" +
                         $"Tables: {salle.NbTables}";

            _dialogService.ShowInformation("Détails de la salle", details);
        }
    }
}
