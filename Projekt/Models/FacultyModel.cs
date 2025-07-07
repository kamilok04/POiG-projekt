using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    /// <summary>
    /// Reprezentuje model wydziału, przechowujący identyfikator oraz nazwę wydziału.
    /// </summary>
    public class FacultyModel : ObservableObject
    {
        private string _id;
        private string _name;

        /// <summary>
        /// Identyfikator wydziału.
        /// </summary>
        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        /// <summary>
        /// Nazwa wydziału.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Tworzy nowy obiekt FacultyModel z pustymi wartościami.
        /// </summary>
        public FacultyModel()
        {
            _id = string.Empty;
            _name = string.Empty;
        }

        /// <summary>
        /// Tworzy nowy obiekt FacultyModel na podstawie identyfikatora i nazwy.
        /// </summary>
        /// <param name="id">Identyfikator wydziału.</param>
        /// <param name="name">Nazwa wydziału.</param>
        public FacultyModel(string id, string name)
        {
            _id = id;
            _name = name;
        }

        /// <summary>
        /// Tworzy nowy obiekt FacultyModel na podstawie słownika danych.
        /// </summary>
        /// <param name="data">Słownik z danymi wydziału.</param>
        public FacultyModel(Dictionary<string, object> data)
        {
            _id = (string)data["nazwa"];
            _name = (string)data["nazwa_krotka"];
        }

        /// <summary>
        /// Zwraca skróconą reprezentację tekstową wydziału.
        /// </summary>
        /// <returns>Nazwa skrócona i identyfikator wydziału.</returns>
        public override string ToString()
        {
            return $"{_name} ({_id})";
        }
    }
}
