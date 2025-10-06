using System.Windows.Input;
using WPF.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace WPF.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel? _currentViewModel;
        private string _statusMessage = "Prêt";
        private DateTime _currentDateTime = DateTime.Now;
        private System.Windows.Threading.DispatcherTimer _timer;

        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public DateTime CurrentDateTime
        {
            get => _currentDateTime;
            set => SetProperty(ref _currentDateTime, value);
        }

        public ICommand NavigateToSalleListCommand { get; }
        public ICommand NavigateToCreateSalleCommand { get; }
        public ICommand NavigateToCreateEtageCommand { get; }

        public MainViewModel()
        {
            NavigateToSalleListCommand = new RelayCommand(NavigateToSalleList);
            NavigateToCreateSalleCommand = new RelayCommand(NavigateToCreateSalle);
            NavigateToCreateEtageCommand = new RelayCommand(NavigateToCreateEtage);

            // Timer pour l'horloge
            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => CurrentDateTime = DateTime.Now;
            _timer.Start();

            // Vue par défaut
            NavigateToSalleList();
        }

        private void NavigateToSalleList()
        {
            CurrentViewModel = App.GetService<SalleListViewModel>();
            StatusMessage = "Liste des salles";
        }

        private void NavigateToCreateSalle()
        {
            CurrentViewModel = App.GetService<CreateSalleViewModel>();
            StatusMessage = "Création d'une nouvelle salle";
        }

        private void NavigateToCreateEtage()
        {
            CurrentViewModel = App.GetService<CreateEtageViewModel>();
            StatusMessage = "Création d'un nouvel étage";
        }
    }
}
