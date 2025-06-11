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
    public class LoginViewModel : IPageViewModel
    {
        string IPageViewModel.Name => "LoginPage";

        #region Fields
        private string _username = "";
        private SecureString _password = new();
        private ICommand _loginCommand;
        private LoginModel _model;
        public event Action? Authenticated;
        #endregion

        #region Constructors
      
        public LoginViewModel()
        {
            _model = new();
            _loginCommand = LoginCommand;
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

        public SecureString Password
        {
            get => _password;
            set
            {
                _password = value;
           
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(
                        param => ValidateLogin());
                }
                return _loginCommand;
            }
        }
        #endregion

        #region Private Methods

        private bool ValidateLogin()
        {
            
            if (String.IsNullOrEmpty(Username) || Password == null)
                return false;

            // connect to db
            // authenticate
            // for now though, it's fine
            if (_model.Authenticated)
            {
                Authenticated?.Invoke();
                return true;
            }
            return false; 

        }


        #endregion


    }
}
