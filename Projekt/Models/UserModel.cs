using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class UserModel
    {
        public string? _name;
        public string? _surname;
        public string? _login;
        public string? _password;
        public string? _email;
        public string? _currentRole;
        public DateTime? _birthDate;
        public int _permissions;
        public int _studentID;

        public UserModel(Dictionary<string, object> data)
        {
            _name = data["imie"] as string;
            _surname = data["nazwisko"] as string;
            _login = data["login"] as string;
            _email = data["email"] as string;
          _birthDate = (DateTime?)data["data_urodzenia"];
            _permissions = (int)data["uprawnienia"];
        }

        public override string ToString()
        {
            return $"{_name} {_surname} ({_login})";
        }

        public override bool Equals(object? obj)
        {
            UserModel? model = obj as UserModel;
            if (model == null) return false;
            if (model._login == null || _login == null) return false;
            return model._login == _login;

        }
    }


}
