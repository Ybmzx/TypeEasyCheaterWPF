using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Navigation;
using TypeEasyCheaterWPF.Services;
using TypeEasyCheaterWPF.ViewModels;

namespace TypeEasyCheaterWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();
        }

        public new static App Current => (App)Application.Current;

        private static IServiceProvider Services;


        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<MainWindowVM>();
            services.AddSingleton<MainWindow>();

            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<IMessageBoxService, MessageBoxService>();
            services.AddSingleton<ITypeEasyCoursesService, TypeEasyCoursesService>();
            services.AddSingleton<ISettingService, XmlSettingService>(x => new XmlSettingService("conf.xml"));
            services.AddSingleton<ITypeEasyProgramMemoryModifyService, TypeEasyProgramMemoryModifyService>();
            services.AddSingleton<ITypeEasyRecordModifyService, TypeEasyRecordModifyService>();

            return services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = Services.GetService<MainWindow>();
            mainWindow!.Show();
        }
    }

}
