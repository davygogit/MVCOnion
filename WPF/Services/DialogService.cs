using System.Windows;

namespace WPF.Services
{
    public interface IDialogService
    {
        void ShowInformation(string title, string message);
        void ShowError(string title, string message);
        void ShowWarning(string title, string message);
        bool ShowConfirmation(string title, string message);
    }

    public class DialogService : IDialogService
    {
        public void ShowInformation(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowWarning(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowConfirmation(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
