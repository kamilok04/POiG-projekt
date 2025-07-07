using Microsoft.Xaml.Behaviors.Media;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    /// <summary>
    /// Model danych kierunku studiów, przechowujący nazwę kierunku.
    /// </summary>
    public class MajorDataModel : ObservableObject
    {
        private string _name;

        /// <summary>
        /// Nazwa kierunku studiów.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Tworzy nową instancję MajorDataModel z podaną nazwą kierunku.
        /// </summary>
        /// <param name="name">Nazwa kierunku studiów.</param>
        public MajorDataModel(string name)
        {
            _name = name;
        }
    }

}
