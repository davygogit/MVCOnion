using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using Domain;
using Infrastructure;
using WPF.ViewModels;
using WPF.Services;

namespace WPF
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    // DbContext
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContext<WebAppMapsContext>(options =>
                        options.UseSqlServer(connectionString));

                    // Repositories
                    services.AddScoped<IRepository<Salle>, Repository<Salle>>();
                    services.AddScoped<IRepository<SallePause>, Repository<SallePause>>();
                    services.AddScoped<IRepository<SalleBubble>, Repository<SalleBubble>>();
                    services.AddScoped<IRepository<SalleReunion>, Repository<SalleReunion>>();
                    services.AddScoped<IRepository<Etage>, Repository<Etage>>();

                    // Services métier (Business Logic Layer)
                    services.AddScoped<IEtageService, EtageService>();

                    // Managers (legacy - à migrer vers Services)
                    services.AddScoped<ISalleManager, SalleManager>();

                    // Services
                    services.AddSingleton<IDialogService, DialogService>();
                    services.AddSingleton<IImageService, ImageService>();

                    // ViewModels
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<SalleListViewModel>();
                    services.AddTransient<EtageViewModel>();
                    services.AddTransient<CreateSalleViewModel>();
                    services.AddTransient<CreateEtageViewModel>();

                    // MainWindow - pas besoin de l'enregistrer, on le crée manuellement
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            // Créer MainWindow manuellement avec DI
            var mainViewModel = _host.Services.GetRequiredService<MainViewModel>();
            var mainWindow = new MainWindow(mainViewModel);
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }

        public static T GetService<T>() where T : class
        {
            return ((App)Current)._host.Services.GetRequiredService<T>();
        }
    }
}
