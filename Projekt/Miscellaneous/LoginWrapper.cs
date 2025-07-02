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

        public bool Valid
        {
            get => _valid ?? throw new ArgumentNullException(nameof(_valid));
            set
            {
                _valid = value;
                OnPropertyChanged(nameof(Valid));
            }
        }
        public string? Token
        {
            get => _token;
            init
            {
                _token = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
        public DatabaseHandler DBHandler
        {
            get => _DBHandler ?? throw new ArgumentNullException();
            init
            {
                _DBHandler = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
        public string? Username
        {
            get => _username;
            init
            {
                _username = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public LoginWrapper(DatabaseHandler DBHandler, string username, string token)
        {
            this.DBHandler = DBHandler;
            Username = username;
            Token = token;
        }

        /// <summary>
        /// Porównuje uprawnienia w bazie danych z wymaganymi.
        /// </summary>
        /// 
        /// <param name="requiredPermissions">Wymagane uprawnienia.</param>
        /// <returns>Czy użytkownik ma żądane uprawnienia</returns>
        public async Task<bool> Authenticate(params int[] requiredPermissions)
        {
            int currentPermissions = await DBHandler.AuthenticateAsync(this);
            if (currentPermissions == 0) Logout(); // https://www.youtube.com/watch?v=GwmJ76VjXaE
            return PermissionHelper.CheckPermissions(currentPermissions, requiredPermissions);
        }


        public void Logout()
        {
            _token = null;
            DestroySession(Username);
            _username = null;
        }

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
