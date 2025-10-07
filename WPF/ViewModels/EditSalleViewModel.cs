using System.Collections.ObjectModel;
using System.Windows.Input;
using Domain;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using WPF.Commands;
using WPF.Services;

namespace WPF.ViewModels
{
    public class EditSalleViewModel : BaseViewModel
    {
        private readonly ISalleService _salleService;
        private readonly IEtageService _etageService;
        private readonly IDialogService _dialogService;
        private readonly IImageService _imageService;
        private readonly Salle _originalSalle;

        private int _salleId;
        private string _nom = string.Empty;
        private int _numero;
        private int? _nbPlaces;
        private int? _nbTables;
        private string _coordonneeX = string.Empty;
        private string _coordonneeY = string.Empty;
        private string? _imagePath;
        private Etage? _selectedEtage;
        private string _selectedTypeSalle = "Reunion";
        private bool _isSaving;

        // Salle Réunion
        private bool _ecran;
        private bool _camera;
        private bool _tableauBlanc;
        private bool _systemeAudio;

        // Salle Pause
        private int _microOndes;
        private int _evier;
        private bool _frigo;
        private bool _distributeur;

        // Salle Bubble
        private bool _priseElectrique;

        private ObservableCollection<Etage> _etages = new();

        public int SalleId
        {
            get => _salleId;
            set => SetProperty(ref _salleId, value);
        }

        public string Nom
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
        }

        public int Numero
        {
            get => _numero;
            set => SetProperty(ref _numero, value);
        }

        public int? NbPlaces
        {
            get => _nbPlaces;
            set => SetProperty(ref _nbPlaces, value);
        }

        public int? NbTables
        {
            get => _nbTables;
            set => SetProperty(ref _nbTables, value);
        }

        public string CoordonneeX
        {
            get => _coordonneeX;
            set => SetProperty(ref _coordonneeX, value);
        }

        public string CoordonneeY
        {
            get => _coordonneeY;
            set => SetProperty(ref _coordonneeY, value);
        }

        public string? ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public Etage? SelectedEtage
        {
            get => _selectedEtage;
            set => SetProperty(ref _selectedEtage, value);
        }

        public string SelectedTypeSalle
        {
            get => _selectedTypeSalle;
            set => SetProperty(ref _selectedTypeSalle, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public ObservableCollection<Etage> Etages
        {
            get => _etages;
            set => SetProperty(ref _etages, value);
        }

        // Salle Réunion
        public bool Ecran
        {
            get => _ecran;
            set => SetProperty(ref _ecran, value);
        }

        public bool Camera
        {
            get => _camera;
            set => SetProperty(ref _camera, value);
        }

        public bool TableauBlanc
        {
            get => _tableauBlanc;
            set => SetProperty(ref _tableauBlanc, value);
        }

        public bool SystemeAudio
        {
            get => _systemeAudio;
            set => SetProperty(ref _systemeAudio, value);
        }

        // Salle Pause
        public int MicroOndes
        {
            get => _microOndes;
            set => SetProperty(ref _microOndes, value);
        }

        public int Evier
        {
            get => _evier;
            set => SetProperty(ref _evier, value);
        }

        public bool Frigo
        {
            get => _frigo;
            set => SetProperty(ref _frigo, value);
        }

        public bool Distributeur
        {
            get => _distributeur;
            set => SetProperty(ref _distributeur, value);
        }

        // Salle Bubble
        public bool PriseElectrique
        {
            get => _priseElectrique;
            set => SetProperty(ref _priseElectrique, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectImageCommand { get; }

        public event EventHandler? SaveCompleted;
        public event EventHandler? CancelRequested;

        public EditSalleViewModel(
            ISalleService salleService,
            IEtageService etageService,
            IDialogService dialogService,
            IImageService imageService,
            Salle salle)
        {
            _salleService = salleService;
            _etageService = etageService;
            _dialogService = dialogService;
            _imageService = imageService;
            _originalSalle = salle;

            SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
            CancelCommand = new RelayCommand(Cancel);
            SelectImageCommand = new RelayCommand(SelectImage);

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Charger les étages (tri automatique par Niveau)
                var etages = await _etageService.GetAllEtagesAsync();
                Etages = new ObservableCollection<Etage>(etages);

                // Charger les données de la salle avec l'étage inclus
                var salle = await _salleService.GetSalleByIdAsync(_originalSalle.Id, includeEtage: true);

                if (salle != null)
                {
                    LoadSalleData(salle);
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError("Erreur", $"Erreur lors du chargement des données: {ex.Message}");
            }
        }

        private void LoadSalleData(Salle salle)
        {
            SalleId = salle.Id;
            Nom = salle.Nom ?? string.Empty;
            Numero = salle.Numero;
            NbPlaces = salle.NbPlaces ?? 0;
            NbTables = salle.NbTables ?? 0;
            CoordonneeX = salle.CoordonneeX ?? string.Empty;
            CoordonneeY = salle.CoordonneeY ?? string.Empty;
            ImagePath = salle.ImgSallePath;
            SelectedEtage = Etages.FirstOrDefault(e => e.Id == salle.EtageId);
            SelectedTypeSalle = salle.TypeSalle.ToString();

            // Charger les données spécifiques selon le type
            if (salle is SalleReunion reunion)
            {
                Ecran = reunion.Ecran;
                Camera = reunion.Camera;
                TableauBlanc = reunion.TableauBlanc;
                SystemeAudio = reunion.SystemeAudio;
            }
            else if (salle is SallePause pause)
            {
                MicroOndes = pause.MicroOndes;
                Evier = pause.Evier;
                Frigo = pause.Frigo;
                Distributeur = pause.Distributeur;
            }
            else if (salle is SalleBubble bubble)
            {
                PriseElectrique = bubble.PriseElectrique;
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Nom) &&
                   Numero > 0 &&
                   SelectedEtage != null &&
                   !IsSaving;
        }

        private async Task SaveAsync()
        {
            if (!CanSave()) return;

            IsSaving = true;
            try
            {
                // Sauvegarde de l'image si modifiée (logique UI-specific)
                string? savedImagePath = ImagePath;
                if (!string.IsNullOrEmpty(ImagePath) && ImagePath != _originalSalle.ImgSallePath)
                {
                    savedImagePath = await _imageService.SaveImageAsync(ImagePath, "Salles");
                }

                // Détermine le type de salle
                TypeSalle typeSalle = Enum.Parse<TypeSalle>(SelectedTypeSalle);

                // Création du DTO pour le service avec toutes les propriétés
                var dto = new UpdateSalleDto
                {
                    Id = SalleId,
                    Nom = Nom,
                    Numero = Numero,
                    NbPlaces = NbPlaces,
                    NbTables = NbTables,
                    CoordonneeX = CoordonneeX,
                    CoordonneeY = CoordonneeY,
                    ImgSallePath = savedImagePath,
                    EtageId = SelectedEtage!.Id,
                    TypeSalle = typeSalle,
                    Favori = _originalSalle.Favori,
                    
                    // Propriétés spécifiques SalleReunion
                    Ecran = Ecran,
                    Camera = Camera,
                    TableauBlanc = TableauBlanc,
                    SystemeAudio = SystemeAudio,
                    
                    // Propriétés spécifiques SallePause
                    MicroOndes = MicroOndes,
                    Evier = Evier,
                    Frigo = Frigo,
                    Distributeur = Distributeur,
                    
                    // Propriétés spécifiques SalleBubble
                    PriseElectrique = PriseElectrique
                };

                // Appel du service (logique métier déléguée avec propriétés spécifiques)
                var updatedSalle = await _salleService.UpdateSalleAsync(dto);

                _dialogService.ShowInformation("Succès", 
                    $"Salle '{updatedSalle.Nom}' (n°{updatedSalle.Numero}) modifiée avec succès !");
                SaveCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (KeyNotFoundException ex)
            {
                _dialogService.ShowError("Erreur", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // Erreur de règle métier (ex: numéro déjà existant)
                _dialogService.ShowError("Règle métier", ex.Message);
            }
            catch (ArgumentException ex)
            {
                // Erreur de validation
                _dialogService.ShowError("Validation", ex.Message);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError("Erreur", $"Erreur lors de la modification: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void Cancel()
        {
            CancelRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SelectImage()
        {
            // Ouvrir un dialogue pour sélectionner une image
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Images (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Tous les fichiers (*.*)|*.*",
                Title = "Sélectionner une image"
            };

            if (dialog.ShowDialog() == true)
            {
                ImagePath = dialog.FileName;
            }
        }
    }
}
