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
        private bool _loginInProgress = false;
        private readonly LoginModel? _model;
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
                    if (_model.Authenticated == LoginModel.LoginOK)
                    {
                        Authenticated?.Invoke();
                    }
                    switch (_model.Authenticated)
                    {
                        case LoginModel.LoginInvalidCredentials:
                            ErrorString = "Niepoprawne dane logowania. Spróbuj ponownie.";
                            break;
                        case LoginModel.LoginSessionExpired:
                            ErrorString = "Sesja wygasła. Proszę zalogować się ponownie.";
                            break;
                        case LoginModel.LoginAccountBlocked:
                            ErrorString = "Konto zostało zablokowane. Skontaktuj się z administratorem.";
                            break;
                        case LoginModel.LoginError:
                            ErrorString = "Wystąpił błąd podczas logowania. Spróbuj ponownie później.";
                            break;
                        default:
                            ErrorString = "Wystąpił nieznany błąd podczas logowania.";
                            break;
                    }
                }
             
            };
        }


        #endregion

       

        #region Public Properties/Commands

        public bool LoginInProgress
        {
            get => _loginInProgress;
            set
            {
                _loginInProgress = value;
                OnPropertyChanged(nameof(LoginInProgress));
            }
        }
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
                        async param => await ValidateLogin(),
                        param => ValidateLoginForm());
                }
                return _loginCommand;
            }
        }

        public LoginWrapper LoginWrapper
        {
            get => _model?.LoginWrapper ?? throw new InvalidOperationException("Model is not initialized.");
            set
            {
                if (_model != null)
                {
                    _model.LoginWrapper = value;
                }
                else
                {
                    throw new InvalidOperationException("Model is not initialized.");
                }
            }
        }
        #endregion

        #region Private Methods

        private async Task<bool> ValidateLogin()
        {

            LoginInProgress = true;
            
            if (_model == null || String.IsNullOrEmpty(Username) || Password == null)
                return false;

            _model.UserName = Username;
            _model.Password = Password;

             ErrorString = "Logowanie w trakcie, proszę czekać...";
            int LoginCode = await _model.GetAuthenticatedAsync();

            LoginInProgress = false;

            if (LoginCode == LoginModel.LoginOK) return true;
    
            return false; 

        }


        private bool ValidateLoginForm()
        {
            return !(
                string.IsNullOrWhiteSpace(Username) ||
                string.IsNullOrWhiteSpace(Password) ||
                LoginInProgress
                );
        }

        #endregion


    }
}
