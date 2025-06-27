using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class PlaceDeleteViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Place Delete";

        public PlaceDeleteViewModel(LoginWrapper loginWrapper)
        {
            
        }
    }
}
