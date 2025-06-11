using Projekt.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();


        }
        public LoginView(LoginViewModel loginViewModel)
        {
            DataContext = loginViewModel;
            InitializeComponent();

        }

        // Nie wiemy nic o widoku modelu, oprócz tego, że zawiera pole Password 
        // MVVM jest całe
        // https://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
                ((dynamic)DataContext).Password = ((PasswordBox)sender).SecurePassword; 
        }
    }
}
