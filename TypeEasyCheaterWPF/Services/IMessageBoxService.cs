using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TypeEasyCheaterWPF.Services
{
    public interface IMessageBoxService
    {
        void Show(string message,
                  string caption,
                  MessageBoxButton? messageBoxButton = null,
                  MessageBoxImage? messageBoxImage = null);
    }
}
