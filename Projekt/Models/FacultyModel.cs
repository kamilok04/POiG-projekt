using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class FacultyModel : ObservableObject
    {
        private string _id;
        private string _name;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public FacultyModel()
        {
            _id = string.Empty;
            _name = string.Empty;
        }

        public FacultyModel(string id, string name)
        {
            _id = id;
            _name = name;
        }

        public FacultyModel(Dictionary<string, object> data)
        {
            _id = (string)data["nazwa"];
            _name = (string)data["nazwa_krotka"];
        }

        public override string ToString()
        {
            return $"{_name} ({_id})";
        }
    }
}
