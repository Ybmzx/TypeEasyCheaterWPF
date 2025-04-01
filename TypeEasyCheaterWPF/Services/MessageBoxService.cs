using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TypeEasyCheaterWPF.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public void Show(string message, string caption, MessageBoxButton? messageBoxButton = null, MessageBoxImage? messageBoxImage = null)
        {
            MessageBox.Show(message, caption, messageBoxButton ?? MessageBoxButton.OK, messageBoxImage ?? MessageBoxImage.Information);
        }
    }
}
