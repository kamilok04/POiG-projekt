using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class UsersEditViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Users Edit";

        public UsersEditViewModel(LoginWrapper loginWrapper)
        {
            
        }
    }
}
