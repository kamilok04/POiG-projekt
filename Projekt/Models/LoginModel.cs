using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Crypto.Engines;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class LoginModel : ObservableObject
    {
        private string _username;
        private string _password;
        private bool _invalidLogin = false;
        private bool _authenticated = false;

        public string UserName { get => _username; set => _username = value; }
        public string Password { get => _password; set => _password = value; }
        public bool Authenticated { get => _authenticated; }
        public DatabaseHandler DBHandler { get; init; } = new();
        private LoginWrapper _loginWrapper { get; set; }
        public LoginWrapper LoginWrapper
        {
            get => _loginWrapper;
            set => _loginWrapper = value;

        }

        public bool InvalidLogin
        {
            get => _invalidLogin;
            set
            {
                _invalidLogin = value;
                OnPropertyChanged(nameof(InvalidLogin));
            }
        }

        public LoginModel()
        {
        }


        public async Task<bool> GetAuthenticatedAsync()
        {
            bool previous = Authenticated;
            await Authenticate();
            if (Authenticated != previous)
                OnPropertyChanged(nameof(Authenticated));
            return _authenticated;
        }

        private async Task Authenticate()
        {
            await DBHandler.ExecuteQueryAsync("SELECT * FROM users WHERE username = @username", new Dictionary<string, object>
                  {
                      { "@username", UserName }
                  }).ContinueWith(task =>
                  {
                      if (task.IsCompletedSuccessfully && task.Result.Count > 0)
                      {
                          foreach (var row in task.Result)
                          {
                              string salt = $@"{(string)row["salt"]}";
                              string localHash = IHashingHandler.GetHashString(Password + salt);
                              string remoteHash = (string)row["hash"];
                              if (localHash.Equals(remoteHash))
                              {
                                  CreateWrapper();
                                  _authenticated = true;
                                  InvalidLogin = false;
                                  return;
                              }
                          }
                          _authenticated = false;
                          InvalidLogin = true;
                      }
                      else
                      {
                          _authenticated = false;
                          InvalidLogin = true;
                      }
                  });
        }



        private void CreateWrapper()
        {
            _loginWrapper = new LoginWrapper(DBHandler, UserName);
            return;
        }
    }

}
