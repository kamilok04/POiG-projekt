
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace Projekt.Models
{
    public class LoginModel : ObservableObject
    {
        private string? _username;
        private string? _password;
        private bool _invalidLogin = false;
        private bool _authenticated = false;
        private string _sessionToken;


        public string? UserName { get => _username; set => _username = value; }
        public string? Password { get => _password; set => _password = value; }
        public bool Authenticated { get => _authenticated; }
        public string SessionToken
        {
            get => _sessionToken;
            set
            {
                _sessionToken = value ?? throw new ArgumentNullException("Token nie jest opcjonalny.");
     
            }
        }

        public DatabaseHandler DBHandler { get; init; } = new();
        private LoginWrapper? _loginWrapper { get; set; }
        public LoginWrapper? LoginWrapper
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
     
            if (await Authenticate() != previous)
                OnPropertyChanged(nameof(Authenticated));
            return Authenticated;
        }

        private async Task<bool> Authenticate()
        {

            string queryString = "SELECT salt, haslo FROM uzytkownik WHERE login = @username";
            await DBHandler.ExecuteQueryAsync(queryString, new Dictionary<string, object>
                  {
                      { "@username", UserName ?? string.Empty}
                  }).ContinueWith(task =>
                  {
                      if (task.IsCompletedSuccessfully && task.Result.Count == 1)
                      {
                          foreach (var row in task.Result)
                          {
                              string salt = $@"{(string)row["salt"]}";
                              string? localHash = IHashingHandler.GetHashString(Password + salt) ?? string.Empty;
                              string remoteHash = (string)row["haslo"];
                              if (localHash.Equals(remoteHash))
                              {
                                   CreateSession().Wait();
                                  CreateWrapper(); // Wrapper wymaga ID sesji
                                  _authenticated = true;
                                  InvalidLogin = false;
                              return;
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

            return _authenticated;
                
        }



        private void CreateWrapper()
        {
            
            _loginWrapper = new LoginWrapper(DBHandler, UserName ?? string.Empty, SessionToken);
            return;
        }

        private async Task CreateSession()
        {
            // Jeśli użytkownik jest zalogowany, kontynuuj jego sesję
            // TODO: nie zezwalaj na przedawnione tokeny
            Dictionary<string, object> param = new() { { "@username", UserName } };

            await DBHandler.ExecuteQueryAsync("SELECT * FROM sesje WHERE login = @username AND data_waznosci > NOW();", param)
                .ContinueWith(async task =>
                {
                    if (task.IsCompletedSuccessfully && task.Result.Count == 1)
                    {
                        var result = task.Result[0];
                        SessionToken = (string)result["token"];
                    }
                    else
                    {
                        // Mamy poprawne logowanie, można nadpisać tokena 
                        // usuń starego

                        // Uboczna konsekwencja: Sesja może być aktywna na jednym urządzeniu naraz
                        // Jak dla mnie to to jest feature

                        await DBHandler.ExecuteNonQueryAsync("DELETE FROM sesje WHERE login = @username;", param);
                    }

                });
            SessionToken ??= Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


            // Corner case: token przedawni się w trakcie logowania
            // Wtedy błąd "niepoprawne logowanie", a druga próba działa, bo usuwa już-starego tokena

            await DBHandler.ExecuteNonQueryAsync(
                "INSERT INTO sesje (login, token, uprawnienia) VALUES (@username, @token, (SELECT uprawnienia FROM uzytkownik WHERE login = @username));"
                , new Dictionary<string, object>
            {
                { "@username", UserName ?? string.Empty },
                { "@token", SessionToken }
            }).ContinueWith(task =>
            {
                if (!(task.IsCompletedSuccessfully && task.Result > 0))
                {
                    // Podwójnego ID sesji nigdy nie będzie, bo login to PK.
                    _authenticated = false;
                    InvalidLogin = true;
                    Console.WriteLine("Problem z sesją!");

                }
               
            });
        }
    }

}
