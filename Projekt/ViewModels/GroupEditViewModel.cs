using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class GroupEditViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Group Edit";

        public GroupEditViewModel(LoginWrapper loginWrapper)
        {
            
        }
        public GroupEditViewModel()
        {

        }
    }
}
