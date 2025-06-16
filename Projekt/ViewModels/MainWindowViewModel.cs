using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class MainWindowViewModel : ConditionalContentControlViewModel
    {
        private LoginWrapper _loginWrapper;
        public LoginWrapper LoginWrapper
        {
            get => _loginWrapper;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _loginWrapper = value;
            }
        }

        public MainWindowViewModel() {
            LoginViewModel loginViewModel = new();
            loginViewModel.Authenticated += () =>
            {
                if (loginViewModel.LoginWrapper != null)
                {
                    LoginWrapper = loginViewModel.LoginWrapper;
                }
                LoginViewModel_Authenticated();
            };
            ChangeViewModel(loginViewModel);
        }

        private void LoginViewModel_Authenticated()
        {

            MainMenuViewModel mainmenu = new(LoginWrapper);

            ChangeViewModel(mainmenu);

        }
    }
}
