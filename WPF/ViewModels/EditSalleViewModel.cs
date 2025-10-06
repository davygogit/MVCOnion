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
        private readonly IRepository<Salle> _salleRepository;
        private readonly IRepository<Etage> _etageRepository;
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
            IRepository<Salle> salleRepository,
            IRepository<Etage> etageRepository,
            IDialogService dialogService,
            IImageService imageService,
            Salle salle)
        {
            _salleRepository = salleRepository;
            _etageRepository = etageRepository;
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
                // Charger les étages
                var etages = await _etageRepository.GetQueryable()
                    .OrderBy(e => e.Niveau)
                    .ToListAsync();
                Etages = new ObservableCollection<Etage>(etages);

                // Charger les données de la salle
                var salle = await _salleRepository.GetQueryable()
                    .Include(s => s.Etage)
                    .FirstOrDefaultAsync(s => s.Id == _originalSalle.Id);

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
                // Récupérer la salle existante
                var salle = await _salleRepository.GetByIdAsync(SalleId);
                if (salle == null)
                {
                    _dialogService.ShowError("Erreur", "Salle introuvable");
                    return;
                }

                // Mettre à jour les propriétés communes
                salle.Nom = Nom;
                salle.Numero = Numero;
                salle.NbPlaces = NbPlaces;
                salle.NbTables = NbTables;
                salle.CoordonneeX = CoordonneeX;
                salle.CoordonneeY = CoordonneeY;
                salle.ImgSallePath = ImagePath;
                salle.EtageId = SelectedEtage!.Id;

                // Mettre à jour les propriétés spécifiques
                if (salle is SalleReunion reunion)
                {
                    reunion.Ecran = Ecran;
                    reunion.Camera = Camera;
                    reunion.TableauBlanc = TableauBlanc;
                    reunion.SystemeAudio = SystemeAudio;
                }
                else if (salle is SallePause pause)
                {
                    pause.MicroOndes = MicroOndes;
                    pause.Evier = Evier;
                    pause.Frigo = Frigo;
                    pause.Distributeur = Distributeur;
                }
                else if (salle is SalleBubble bubble)
                {
                    bubble.PriseElectrique = PriseElectrique;
                }

                await _salleRepository.UpdateAsync(salle);
                await _salleRepository.SaveChangesAsync();

                _dialogService.ShowInformation("Succès", "Salle modifiée avec succès");
                SaveCompleted?.Invoke(this, EventArgs.Empty);
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
