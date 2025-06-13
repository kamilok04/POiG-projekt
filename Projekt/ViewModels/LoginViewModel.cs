using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class LoginViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "LoginPage";

        #region Fields
        private string _username = "";
        private string _password = "";
        private string _errorString = "";
        private ICommand _loginCommand;
        private LoginModel? _model;
        public event Action? Authenticated;
        #endregion

        #region Constructors
      
        public LoginViewModel() 
        {
            _loginCommand = LoginCommand;
            _model = new();
            _model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(LoginModel.Authenticated))
                {
                    if (_model.Authenticated)
                    {
                        Authenticated?.Invoke();
                    }
                }
                if (e.PropertyName == nameof(LoginModel.InvalidLogin))
                {
                    if (_model.InvalidLogin) {
                        // Handle invalid login, e.g., show a message to the user
                        // This could be a property that the view binds to for displaying an error message
                        ErrorString = "Niepoprawne dane logowania. Spróbuj ponownie.";
                    }
                }
            };
        }


        #endregion

       

        #region Public Properties/Commands
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
               
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
           
            }
        }

        public string ErrorString
        {
            get => _errorString;
            set
            {
                _errorString = value;
                OnPropertyChanged(nameof(ErrorString));

            }
        }

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(
                        async param => await ValidateLogin());
                }
                return _loginCommand;
            }
        }
        #endregion

        #region Private Methods

        private async Task<bool> ValidateLogin()
        {
            
            if (String.IsNullOrEmpty(Username) || Password == null)
                return false;

            _model.UserName = Username;
            _model.Password = Password;

             ErrorString = "Logowanie w trakcie, proszę czekać...";
            bool validLogin = await _model.GetAuthenticatedAsync();



            if (validLogin)
            {
            
                return true;

            }
            return false; 

        }


        #endregion


    }
}
