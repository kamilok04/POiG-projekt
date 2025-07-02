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
        private LoginWrapper? _loginWrapper;
        private MainMenuViewModel mainmenu;
        public LoginWrapper? LoginWrapper
        {
            get => _loginWrapper;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                _loginWrapper = value;
                _loginWrapper.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(LoginWrapper.Valid))
                    {
                        if(LoginWrapper == null || !LoginWrapper.Valid)
                        {
                            
                            NewLogin();
                        }
                      
                    }
                };  
            }
        }

        public MainWindowViewModel() {
            NewLogin();

        }

        private void LoginViewModel_Authenticated()
        {
            if(LoginWrapper != null)
            {
                mainmenu = new(LoginWrapper);

                ChangeViewModel(mainmenu);
            }
        }

        private void NewLogin()
        {
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

        public void SetMainMenuItem(IPageViewModel pageViewModel)
        {
            if (pageViewModel == null)
                throw new ArgumentNullException(nameof(pageViewModel));
            mainmenu.ChangeViewModel(pageViewModel);
        }
    }
}
