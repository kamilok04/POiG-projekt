using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Miscellaneous
{
    public class LoginWrapper : ObservableObject
    {
        private DatabaseHandler? _DBHandler;
        private string? _username;
        private string? _token;
        private bool? _valid;

        /// <summary>
        /// Flaga informująca o poprawności sesji.
        /// </summary>
        public bool Valid
        {
            get => _valid ?? throw new ArgumentNullException(nameof(_valid));
            set
            {
                _valid = value;
                OnPropertyChanged(nameof(Valid));
            }
        }

        /// <summary>
        /// Token autoryzacyjny użytkownika.
        /// </summary>
        public string? Token
        {
            get => _token;
            init
            {
                _token = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Handler bazy danych powiązany z użytkownikiem.
        /// </summary>
        public DatabaseHandler DBHandler
        {
            get => _DBHandler ?? throw new ArgumentNullException();
            init
            {
                _DBHandler = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Nazwa użytkownika.
        /// </summary>
        public string? Username
        {
            get => _username;
            init
            {
                _username = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Inicjalizuje nową instancję klasy LoginWrapper z podanym handlerem bazy, nazwą użytkownika i tokenem.
        /// </summary>
        /// <param name="DBHandler">Handler bazy danych.</param>
        /// <param name="username">Nazwa użytkownika.</param>
        /// <param name="token">Token autoryzacyjny.</param>
        public LoginWrapper(DatabaseHandler DBHandler, string username, string token)
        {
            this.DBHandler = DBHandler;
            Username = username;
            Token = token;
        }

        /// <summary>
        /// Sprawdza, czy użytkownik posiada wymagane uprawnienia.
        /// </summary>
        /// <param name="requiredPermissions">Wymagane uprawnienia.</param>
        /// <returns>True, jeśli użytkownik ma żądane uprawnienia.</returns>
        public async Task<bool> Authenticate(params int[] requiredPermissions)
        {
            int currentPermissions = await DBHandler.AuthenticateAsync(this);
            if (currentPermissions == 0) Logout(); // https://www.youtube.com/watch?v=GwmJ76VjXaE
            return PermissionHelper.CheckPermissions(currentPermissions, requiredPermissions);
        }

        /// <summary>
        /// Wylogowuje użytkownika i zamyka sesję.
        /// </summary>
        public void Logout()
        {
            _token = null;
            DestroySession(Username);
            _username = null;
        }

        /// <summary>
        /// Niszczy sesję użytkownika (prywatna metoda asynchroniczna).
        /// </summary>
        /// <param name="username">Nazwa użytkownika.</param>
        private async void DestroySession(string? username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                if (_DBHandler == null)
                    throw new InvalidOperationException("DBHandler is not initialized.");
                await _DBHandler.DestroySession(username);
                _DBHandler = null;
            }
           
        
            Valid = false;
            
        }

    }
}
