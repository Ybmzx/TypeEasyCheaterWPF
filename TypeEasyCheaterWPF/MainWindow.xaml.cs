using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TypeEasyCheaterWPF.ViewModels;

namespace TypeEasyCheaterWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowVM ViewModel { get; }
        public MainWindow(MainWindowVM vm)
        {
            InitializeComponent();
            DataContext = this;
            ViewModel = vm;
        }
    }
}