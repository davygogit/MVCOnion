using System.Windows;
using WPF.ViewModels;

namespace WPF.Windows
{
    public partial class EditSalleWindow : Window
    {
        public EditSalleWindow(EditSalleViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            // S'abonner aux événements du ViewModel
            viewModel.SaveCompleted += (s, e) =>
            {
                DialogResult = true;
                Close();
            };

            viewModel.CancelRequested += (s, e) =>
            {
                DialogResult = false;
                Close();
            };
        }
    }
}
