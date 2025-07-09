
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cmp;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
        public const int LoginOK = 0;
        public const int LoginInvalidCredentials = 1;
        public const int LoginSessionExpired = 2;
        public const int LoginError = 3;
        public const int LoginAccountBlocked = 4;
        public const int LoginOffline = 5;

        public const int LoginUndefined = -1;

        private string? _username;
        private string? _password;
       
        private int _authenticated = LoginUndefined;
        private string? _sessionToken;


        public string? UserName { get => _username; set => _username = value; }
        public string? Password { get => _password; set => _password = value; }
        public int Authenticated { get => _authenticated; }
        public string? SessionToken
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

     
        public LoginModel()
        {
        }


        public async Task<int> GetAuthenticatedAsync()
        {
            int previous = Authenticated;
            int LoginResult = await Authenticate();
           
            OnPropertyChanged(nameof(Authenticated));
            
            
            return Authenticated;
        }

        private async Task<int> Authenticate()
        {
            string queryString = "SELECT salt, haslo FROM uzytkownik WHERE login = @username";
            await DBHandler.ExecuteQueryAsync(queryString, new Dictionary<string, object>
                  {
                      { "@username", UserName ?? string.Empty }
                  }).ContinueWith(async task =>
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
                                  int sessionResult = CreateSession().Result; // Tu trzeba zablokować wątek!

                                  if (sessionResult == LoginAccountBlocked)
                                  {
                                      _authenticated = LoginAccountBlocked;
                                      return;
                                  }
                                  CreateWrapper(); // Wrapper wymaga ID sesji
                                  _authenticated = LoginOK;
                                  return;
                              }
                              _authenticated = LoginInvalidCredentials;
                              return;
                          }
                      }
                      else
                      {
                          Exception? ex = task.Exception;
                          if (ex == null) return;
                          ex = ex.InnerException;
                          if (ex == null) return;
                          var data = ex.Data;
                          if (data == null) return;
                          if ((MySqlErrorCode?) data["Server Error Code"] == MySql.Data.MySqlClient.MySqlErrorCode.UnableToConnectToHost)
                              _authenticated = LoginOffline;
                          else 
                              _authenticated = LoginError;
                      }
                  });

            return _authenticated;
        }



        private void CreateWrapper()
        {
            
            _loginWrapper = new LoginWrapper(DBHandler, UserName ?? string.Empty, SessionToken ?? string.Empty);
            return;
        }

        private async Task<int> CreateSession()
        {
            if (UserName == null) return LoginError;

            // Wyproś zablokowanych użytkowników

            int permissions = await DBHandler.ExecuteQueryAsync("SELECT uprawnienia FROM uzytkownik WHERE login = @username;", new() { { "@username", UserName } })
             .ContinueWith(task =>
             {
                 if (task.IsCompletedSuccessfully && task.Result.Count == 1)
                 {
                     return (int)task.Result[0]["uprawnienia"];
                 }
                 return 0; // Brak uprawnień, nie powinno się zdarzyć
             });

            if (permissions == 0)
            {
                _authenticated = LoginAccountBlocked;
                Console.WriteLine("Brak uprawnień do zalogowania się.");
                return LoginAccountBlocked;
            }

            int loginState = LoginUndefined;


            // Jeśli użytkownik jest zalogowany, kontynuuj jego sesję
            Dictionary<string, object> param = new() { { "@username", UserName ?? string.Empty } };

            await DBHandler.ExecuteQueryAsync("SELECT * FROM sesje WHERE login = @username AND data_waznosci > NOW();", param)
                .ContinueWith(async task =>
                {
                    if (task.IsCompletedSuccessfully && task.Result.Count == 1)
                    {
                        var result = task.Result[0];
                        SessionToken = (string)result["token"];
                        loginState = LoginOK;
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

            if (loginState == LoginOK) return LoginOK; // kontynuuj aktywną sesję

            SessionToken ??= Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));


         
            // Corner case: token przedawni się w trakcie logowania
            // Wtedy błąd "niepoprawne logowanie", a druga próba działa, bo usuwa już-starego tokena


            await DBHandler.ExecuteNonQueryAsync(
                "INSERT INTO sesje (login, token, uprawnienia) VALUES (@username, @token, @permissions);"
                , new Dictionary<string, object>
            {
                { "@username", UserName ?? string.Empty },
                { "@token", SessionToken },
                    {"@permissions", permissions }
            }).ContinueWith(task =>
            {
                if (!(task.IsCompletedSuccessfully && task.Result > 0))
                {
                    // Podwójnego ID sesji nigdy nie będzie, bo login to PK.
                    _authenticated = LoginError;
                    Console.WriteLine("Problem z sesją!");
                    loginState = LoginError;
                   
                }
               
            });
            return loginState;
        }

    }

}
