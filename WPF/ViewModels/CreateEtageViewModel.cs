using System.Windows.Input;
using Domain;
using Microsoft.Win32;
using WPF.Commands;
using WPF.Services;

namespace WPF.ViewModels
{
    public class CreateEtageViewModel : BaseViewModel
    {
        private readonly IRepository<Etage> _etageRepository;
        private readonly IDialogService _dialogService;
        private readonly IImageService _imageService;

        private int _niveau;
        private string _nom = string.Empty;
        private string? _imagePath;
        private bool _isSaving;

        public int Niveau
        {
            get => _niveau;
            set => SetProperty(ref _niveau, value);
        }

        public string Nom
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
        }

        public string? ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public ICommand SelectImageCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateEtageViewModel(
            IRepository<Etage> etageRepository,
            IDialogService dialogService,
            IImageService imageService)
        {
            _etageRepository = etageRepository;
            _dialogService = dialogService;
            _imageService = imageService;

            SelectImageCommand = new RelayCommand(SelectImage);
            SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SelectImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|Tous les fichiers|*.*",
                Title = "Sélectionner le plan de l'étage"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Nom) && !IsSaving;
        }

        private async Task SaveAsync()
        {
            IsSaving = true;
            try
            {
                string? savedImagePath = null;
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    savedImagePath = await _imageService.SaveImageAsync(ImagePath, "PlansEtages");
                }

                var etage = new Etage
                {
                    Niveau = Niveau,
                    Nom = Nom,
                    ImgPlanEtagePath = savedImagePath
                };

                await _etageRepository.AddAsync(etage);
                await _etageRepository.SaveChangesAsync();

                _dialogService.ShowInformation("Succès", "Étage créé avec succès !");
                
                // Reset form
                Niveau = 0;
                Nom = string.Empty;
                ImagePath = null;
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
            Niveau = 0;
            Nom = string.Empty;
            ImagePath = null;
        }
    }
}
