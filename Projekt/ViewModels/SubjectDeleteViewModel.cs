using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;

namespace Projekt.ViewModels
{
    public class SubjectDeleteViewModel : ObservableObject, IPageViewModel
    {
        public string Name => "Subject Delete";

        public SubjectDeleteViewModel(LoginWrapper loginWrapper)
        {
            
        }
    }
}
