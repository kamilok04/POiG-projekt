using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace Projekt.ViewModels
{
    public class TimeTableViewModel : ObservableObject, IPageViewModel
    {
        public TimeTableViewModel() { }
        private List<string> _daysOfWeek = new List<string> { "poniedziałek", "wtorek", "środa", "czwartek", "piątek", "sobota", "niedziela" };

        public NotifyTaskCompletion<Dictionary<string, List<LessonModel>>> TimeTable { get; set; }
        string IPageViewModel.Name => nameof(TimeTableViewModel);

        private  TimeTableModel Model { get; init; }
        private LoginWrapper Wrapper { get; init; }

        public TimeTableViewModel(LoginWrapper wrapper)
        {
            Wrapper = wrapper;
            Model = new(wrapper);
            TimeTable = new(Model.GetTimeTable(_daysOfWeek));

        }
        
  
        
    }
}
