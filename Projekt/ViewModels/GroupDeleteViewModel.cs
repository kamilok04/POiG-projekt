using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class GroupDeleteViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Group Delete";

        public GroupDeleteViewModel(LoginWrapper loginWrapper)
        {
            
        }
    }
}
