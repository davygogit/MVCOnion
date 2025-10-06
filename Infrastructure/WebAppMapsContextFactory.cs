using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Infrastructure
{
    public class WebAppMapsContextFactory : IDesignTimeDbContextFactory<WebAppMapsContext>
    {
        public WebAppMapsContext CreateDbContext(string[] args)
        {
            // Charger la configuration depuis le projet Web ou WPF
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            // Si pas trouvé, essayer depuis le dossier WPF
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                var wpfPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "WPF");
                if (Directory.Exists(wpfPath))
                {
                    configuration = new ConfigurationBuilder()
                        .SetBasePath(wpfPath)
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
                    connectionString = configuration.GetConnectionString("DefaultConnection");
                }
            }

            // Si toujours pas trouvé, essayer depuis le dossier Web
            if (string.IsNullOrEmpty(connectionString))
            {
                var webPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Web");
                if (Directory.Exists(webPath))
                {
                    configuration = new ConfigurationBuilder()
                        .SetBasePath(webPath)
                        .AddJsonFile("appsettings.json", optional: false)
                        .Build();
                    connectionString = configuration.GetConnectionString("DefaultConnection");
                }
            }

            // Fallback: utiliser LocalDB par défaut
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=WebAppMaps;Trusted_Connection=true;TrustServerCertificate=true;";
            }

            var optionsBuilder = new DbContextOptionsBuilder<WebAppMapsContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new WebAppMapsContext(optionsBuilder.Options);
        }
    }
}
