using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class LessonsDeleteViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Lessons Delete";

        public LessonsDeleteViewModel(LoginWrapper loginWrapper)
        {

        }
        public LessonsDeleteViewModel()
        {

        }
    }
}
