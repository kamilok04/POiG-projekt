using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class UsersDeleteViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Users Delete";

        public UsersDeleteViewModel(LoginWrapper loginWrapper)
        {
            
        }
    }
}
