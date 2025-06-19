using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class LessonsCreateViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "LessonsCreate";
        // for designer only
        public LessonsCreateViewModel() { }

        #region Fields

        //private ObservableCollection<Group>? _groups;
        //private Group? _selectedGroup;
        //private ObservableCollection<Subject>? _subjects;
        //private Subject? _selectedSubject; 
        private List<string> _types = new List<string> { "Wykład", "Ćwiczenia", "Laboratoria", "Seminarium" };
        private string? _selectedType;
        //private ObservableCollection<Place>? _places;
        //private Place? _selectedTeacher;
        private List<string> _daysOfWeek = new List<string> { "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };
        private string? _selectedDayOfWeek;
        private string? _startTime;
        private string? _endTime;
        private LessonsCreateModel _lessonsCreateModel;

        #endregion

        #region Public Properties/Commands


        public List<string> Types { get => _types; }
        public string? SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged(nameof(SelectedType));
                }
            }
        }
        public List<string> DaysOfWeek { get => _daysOfWeek; }

        public string? SelectedDayOfWeek
        {
            get => _selectedDayOfWeek;
            set
            {
                if (_selectedDayOfWeek != value)
                {
                    _selectedDayOfWeek = value;
                    OnPropertyChanged(nameof(SelectedDayOfWeek));
                }
            }
        }

        public string? StartTime
        {
            get => _startTime;
            set
            {
                if (_startTime != value)
                {
                    _startTime = value;
                    OnPropertyChanged(nameof(StartTime));
                }
            }
        }

        public string? EndTime
        {
            get => _endTime;
            set
            {
                if (_endTime != value)
                {
                    _endTime = value;
                    OnPropertyChanged(nameof(EndTime));
                }
            }
        }

        public LessonsCreateModel LessonsCreateModel { get => _lessonsCreateModel; set => _lessonsCreateModel = value; }

        public LessonsCreateViewModel(LoginWrapper loginWrapper)
        {
            LessonsCreateModel = new (loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }

        #endregion

        #region Private Methods

        private bool IsValidTimeFormat(string time)
        {
            // Regex to match HH:MM format
            return Regex.IsMatch(time, @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
        }

        #endregion
    }

}
