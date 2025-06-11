using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class LoginModel
    {
        private readonly string _username;
        private bool _authenticated;
        public string UserName { get => _username; init => _username = value; }
        public bool Authenticated
        {
            get
            {
                Authenticate(); 
                return _authenticated;
            }
        }
        public void Authenticate() { _authenticate(); }

        private void _authenticate()
        {
            // connect to DB
            // authenticate
            _authenticated = true;

            return;
        }

    }


}
