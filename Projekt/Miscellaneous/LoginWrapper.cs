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
        private DatabaseHandler _DBHandler { get; init; }
        private string _username { get; init; }
        public DatabaseHandler DBHandler
        {
            get => _DBHandler;
            init
            {
                _DBHandler = value ?? throw new ArgumentNullException(nameof(value));
            }
        }
        public string Username
        {
            get => _username;
            init
            {
                _username = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        public LoginWrapper(DatabaseHandler DBHandler, string username)
        {
            this.DBHandler = DBHandler ?? throw new ArgumentNullException(nameof(DBHandler));
            Username = username ?? throw new ArgumentNullException(nameof(username));
        }


    }
}
