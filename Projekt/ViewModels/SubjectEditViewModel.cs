using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class SubjectEditViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Subject Edit";

        public SubjectEditViewModel(LoginWrapper loginWrapper)
        {
            
        }
    }
}
