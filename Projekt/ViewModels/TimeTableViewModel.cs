using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class TimeTableViewModel(LoginWrapper wrapper)
    {

        private List<string> _daysOfWeek = new List<string> { "poniedziałek", "wtorek", "środa", "czwartek", "piątek", "sobota", "niedziela" };

        private DatabaseHandler DBHandler = wrapper.DBHandler;
        private async void GetTimeTables()
        {
            
            foreach(string day in _daysOfWeek)
            {

            }
        }
    }
}
