using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class LessonsEditViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Lessons Edit";

        public LessonsEditViewModel(LoginWrapper loginWrapper)
        {
            
        }
    }
}
