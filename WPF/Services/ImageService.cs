using System.IO;
using System.Threading.Tasks;

namespace WPF.Services
{
    public interface IImageService
    {
        Task<string?> SaveImageAsync(string sourcePath, string subfolder);
        string? GetImagePath(string? relativePath);
    }

    public class ImageService : IImageService
    {
        private readonly string _assetsBasePath;

        public ImageService()
        {
            _assetsBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets");
            
            // Créer les dossiers si nécessaire
            Directory.CreateDirectory(Path.Combine(_assetsBasePath, "Salles"));
            Directory.CreateDirectory(Path.Combine(_assetsBasePath, "PlansEtages"));
        }

        public async Task<string?> SaveImageAsync(string sourcePath, string subfolder)
        {
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
                return null;

            try
            {
                var fileName = Path.GetFileName(sourcePath);
                var destinationFolder = Path.Combine(_assetsBasePath, subfolder);
                var destinationPath = Path.Combine(destinationFolder, fileName);

                // Créer le dossier si nécessaire
                Directory.CreateDirectory(destinationFolder);

                // Copier le fichier
                using (var sourceStream = File.OpenRead(sourcePath))
                using (var destinationStream = File.Create(destinationPath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                // Retourner le chemin relatif
                return $"assets/{subfolder}/{fileName}";
            }
            catch
            {
                return null;
            }
        }

        public string? GetImagePath(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            return File.Exists(fullPath) ? fullPath : null;
        }
    }
}
