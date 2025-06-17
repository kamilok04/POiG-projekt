using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Miscellaneous
{
    public class LoginWrapper
    {
        private DatabaseHandler? _DBHandler;
        private string? _username;
        private string? _token;
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
            this.DBHandler = DBHandler ?? throw new ArgumentNullException(nameof(DBHandler));
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        public async Task<bool> Authenticate()
        {
            return await DBHandler.AuthenticateAsync(this);
        }


    }
}
