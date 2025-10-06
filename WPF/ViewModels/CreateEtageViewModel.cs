using System.Windows.Input;
using Domain;
using Microsoft.Win32;
using WPF.Commands;
using WPF.Services;

namespace WPF.ViewModels
{
    public class CreateEtageViewModel : BaseViewModel
    {
        private readonly IEtageService _etageService;
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
            IEtageService etageService,
            IDialogService dialogService,
            IImageService imageService)
        {
            _etageService = etageService;
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
                // Sauvegarde de l'image (logique UI-specific)
                string? savedImagePath = null;
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    savedImagePath = await _imageService.SaveImageAsync(ImagePath, "PlansEtages");
                }

                // Création du DTO pour le service
                var dto = new CreateEtageDto
                {
                    Niveau = Niveau,
                    Nom = Nom,
                    ImgPlanEtagePath = savedImagePath
                };

                // Appel du service (logique métier déléguée)
                var etage = await _etageService.CreateEtageAsync(dto);

                _dialogService.ShowInformation("Succès", 
                    $"Étage '{etage.Nom}' (niveau {etage.Niveau}) créé avec succès !");
                
                // Reset form
                Niveau = 0;
                Nom = string.Empty;
                ImagePath = null;
            }
            catch (InvalidOperationException ex)
            {
                // Erreur de règle métier (ex: niveau déjà existant)
                _dialogService.ShowError("Règle métier", ex.Message);
            }
            catch (ArgumentException ex)
            {
                // Erreur de validation
                _dialogService.ShowError("Validation", ex.Message);
            }
            catch (Exception ex)
            {
                // Erreur technique imprévue
                _dialogService.ShowError("Erreur", 
                    $"Erreur lors de la création de l'étage: {ex.Message}");
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
