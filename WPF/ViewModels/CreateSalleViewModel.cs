using System.Collections.ObjectModel;
using System.Windows.Input;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using WPF.Commands;
using WPF.Services;

namespace WPF.ViewModels
{
    public class CreateSalleViewModel : BaseViewModel
    {
        private readonly IRepository<Salle> _salleRepository;
        private readonly IRepository<Etage> _etageRepository;
        private readonly IDialogService _dialogService;
        private readonly IImageService _imageService;

        private ObservableCollection<Etage> _etages = new();
        private int _numero;
        private string _nom = string.Empty;
        private Etage? _selectedEtage;
        private string? _imagePath;
        private TypeSalle _selectedTypeSalle = TypeSalle.Reunion;
        private string _coordonneeX = "0";
        private string _coordonneeY = "0";
        private int? _nbTables;
        private int? _nbPlaces;
        private bool _isSaving;

        // SalleReunion
        private bool _ecran;
        private bool _camera;
        private bool _tableauBlanc;
        private bool _systemeAudio;

        // SallePause
        private int _microOndes;
        private bool _frigo;
        private int _evier;
        private bool _distributeur;

        // SalleBubble
        private bool _priseElectrique;

        public ObservableCollection<Etage> Etages
        {
            get => _etages;
            set => SetProperty(ref _etages, value);
        }

        public int Numero
        {
            get => _numero;
            set => SetProperty(ref _numero, value);
        }

        public string Nom
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
        }

        public Etage? SelectedEtage
        {
            get => _selectedEtage;
            set => SetProperty(ref _selectedEtage, value);
        }

        public string? ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public TypeSalle SelectedTypeSalle
        {
            get => _selectedTypeSalle;
            set => SetProperty(ref _selectedTypeSalle, value);
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

        public int? NbTables
        {
            get => _nbTables;
            set => SetProperty(ref _nbTables, value);
        }

        public int? NbPlaces
        {
            get => _nbPlaces;
            set => SetProperty(ref _nbPlaces, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        // SalleReunion Properties
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

        // SallePause Properties
        public int MicroOndes
        {
            get => _microOndes;
            set => SetProperty(ref _microOndes, value);
        }

        public bool Frigo
        {
            get => _frigo;
            set => SetProperty(ref _frigo, value);
        }

        public int Evier
        {
            get => _evier;
            set => SetProperty(ref _evier, value);
        }

        public bool Distributeur
        {
            get => _distributeur;
            set => SetProperty(ref _distributeur, value);
        }

        // SalleBubble Properties
        public bool PriseElectrique
        {
            get => _priseElectrique;
            set => SetProperty(ref _priseElectrique, value);
        }

        public ICommand LoadEtagesCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateSalleViewModel(
            IRepository<Salle> salleRepository,
            IRepository<Etage> etageRepository,
            IDialogService dialogService,
            IImageService imageService)
        {
            _salleRepository = salleRepository;
            _etageRepository = etageRepository;
            _dialogService = dialogService;
            _imageService = imageService;

            LoadEtagesCommand = new AsyncRelayCommand(LoadEtagesAsync);
            SelectImageCommand = new RelayCommand(SelectImage);
            SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
            CancelCommand = new RelayCommand(Cancel);

            _ = LoadEtagesAsync();
        }

        private async Task LoadEtagesAsync()
        {
            try
            {
                // Utilisation de GetQueryable() pour ajouter OrderBy
                var etages = await _etageRepository.GetQueryable()
                    .OrderBy(e => e.Niveau)
                    .ToListAsync();
                
                Etages = new ObservableCollection<Etage>(etages);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError("Erreur", $"Erreur lors du chargement des étages: {ex.Message}");
            }
        }

        private void SelectImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|Tous les fichiers|*.*",
                Title = "Sélectionner une image de la salle"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }

        private bool CanSave()
        {
            return SelectedEtage != null && !string.IsNullOrWhiteSpace(Nom) && !IsSaving;
        }

        private async Task SaveAsync()
        {
            if (SelectedEtage == null) return;

            IsSaving = true;
            try
            {
                string savedImagePath = "assets/Salles/DefautSalle.png";
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    var saved = await _imageService.SaveImageAsync(ImagePath, "Salles");
                    if (!string.IsNullOrEmpty(saved))
                        savedImagePath = saved;
                }

                Salle salle = SelectedTypeSalle switch
                {
                    TypeSalle.Reunion => new SalleReunion
                    {
                        Numero = Numero,
                        Nom = Nom,
                        EtageId = SelectedEtage.Id,
                        ImgSallePath = savedImagePath,
                        TypeSalle = SelectedTypeSalle,
                        CoordonneeX = CoordonneeX,
                        CoordonneeY = CoordonneeY,
                        NbTables = NbTables,
                        NbPlaces = NbPlaces,
                        Ecran = Ecran,
                        Camera = Camera,
                        TableauBlanc = TableauBlanc,
                        SystemeAudio = SystemeAudio
                    },
                    TypeSalle.Pause => new SallePause
                    {
                        Numero = Numero,
                        Nom = Nom,
                        EtageId = SelectedEtage.Id,
                        ImgSallePath = savedImagePath,
                        TypeSalle = SelectedTypeSalle,
                        CoordonneeX = CoordonneeX,
                        CoordonneeY = CoordonneeY,
                        NbTables = NbTables,
                        NbPlaces = NbPlaces,
                        MicroOndes = MicroOndes,
                        Frigo = Frigo,
                        Evier = Evier,
                        Distributeur = Distributeur
                    },
                    TypeSalle.Bubble => new SalleBubble
                    {
                        Numero = Numero,
                        Nom = Nom,
                        EtageId = SelectedEtage.Id,
                        ImgSallePath = savedImagePath,
                        TypeSalle = SelectedTypeSalle,
                        CoordonneeX = CoordonneeX,
                        CoordonneeY = CoordonneeY,
                        NbTables = NbTables,
                        NbPlaces = NbPlaces,
                        PriseElectrique = PriseElectrique
                    },
                    _ => new Salle
                    {
                        Numero = Numero,
                        Nom = Nom,
                        EtageId = SelectedEtage.Id,
                        ImgSallePath = savedImagePath,
                        TypeSalle = SelectedTypeSalle,
                        CoordonneeX = CoordonneeX,
                        CoordonneeY = CoordonneeY,
                        NbTables = NbTables,
                        NbPlaces = NbPlaces
                    }
                };

                await _salleRepository.AddAsync(salle);
                await _salleRepository.SaveChangesAsync();

                _dialogService.ShowInformation("Succès", "Salle créée avec succès !");
                
                Cancel();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError("Erreur", $"Erreur lors de la création: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private void Cancel()
        {
            Numero = 0;
            Nom = string.Empty;
            SelectedEtage = null;
            ImagePath = null;
            CoordonneeX = "0";
            CoordonneeY = "0";
            NbTables = null;
            NbPlaces = null;
            Ecran = false;
            Camera = false;
            TableauBlanc = false;
            SystemeAudio = false;
            MicroOndes = 0;
            Frigo = false;
            Evier = 0;
            Distributeur = false;
            PriseElectrique = false;
        }
    }
}
