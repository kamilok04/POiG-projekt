using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class CoordinatorModel(Dictionary<string, object> data) : UserModel(data)
    {

        public string _title = data["tytul"] as string ?? string.Empty;

        public override string ToString()
        {
            return $"{_title} {_name} {_surname} ({_login})";
        }

    }
}
