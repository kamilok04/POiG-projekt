using System.Configuration;
using System.Data;
using System.Windows;

namespace Projekt
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
       {
            base.OnStartup(e);

            Projekt.Views.MainWindowView app = new();
            Projekt.ViewModels.MainWindowViewModel context = new();
            app.DataContext = context;
            app.Show();
        }
    }

}
