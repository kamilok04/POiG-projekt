using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Projekt.ViewModels
{
    public class LessonsCreateViewModel :
           ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "LessonsCreate";
        // for designer only  

        #region Fields  

        public ObservableCollection<GroupEditModel> Groups { get; set; } = new();
        private GroupEditModel? _selectedGroup;
        public ObservableCollection<SubjectModel> Subjects { get; set; } = new();
        private SubjectModel? _selectedSubject;
        private List<string> _types = new List<string> { "Wykład", "Ćwiczenia", "Laboratoria", "Seminarium" };
        private string? _selectedType;
        public ObservableCollection<PlaceModel> Places { get; set; } = new();
        private PlaceModel? _selectedPlace;
        private List<string> _daysOfWeek = new List<string> { "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };
        private string? _selectedDayOfWeek;
  
        private TimeOnly? _startTime;
        private TimeOnly? _endTime;
        private string? _successString = "";

        private string? _errorString= "";
      

        private LessonsCreateModel Model { get; init; }
        private LoginWrapper wrapper;

        #endregion

        #region Constructor  

        public LessonsCreateViewModel(LoginWrapper loginWrapper)
        {
            wrapper = loginWrapper;
            Model = new LessonsCreateModel(loginWrapper);


        }



        #endregion

        #region Public Properties/Commands
        public string SuccessString
        {
            get => _successString ?? "";
            set
            {
                _successString = value;
                OnPropertyChanged(nameof(SuccessString));
            }
        }

        public string ErrorString
        {
            get => _errorString ?? "";
            set
            {
                _errorString = value;
                OnPropertyChanged(nameof(ErrorString));
            }
        }

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

        public GroupEditModel? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
            }
        }

        public PlaceModel? SelectedPlace
        {
            get => _selectedPlace;
            set
            {
                _selectedPlace = value;
                OnPropertyChanged(nameof(SelectedPlace));
            }


        }

        public SubjectModel? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged(nameof(SelectedSubject));
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

        public TimeOnly? StartTime
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

        public TimeOnly? EndTime
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

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get => _saveCommand ??= new RelayCommand(
                async param => await SaveAsync(),
                param => IsFormValid()
                );
        }

        private ICommand _cancelCommand;
        public ICommand CancelCommand
        {
            get => _cancelCommand ??= new RelayCommand(
                param => Cancel());
        }

        #endregion

        #region Private Methods  


        private async Task SaveAsync()
        {
            bool success = await Model.SaveAsync(SelectedGroup, SelectedSubject, SelectedPlace, SelectedType, SelectedDayOfWeek, StartTime, EndTime);
            if (success)
            {
                SuccessString = "Dodano pomyślnie!";
                ErrorString = "";
            }
            else
            {
                SuccessString = "";
                ErrorString = "Błąd podczas dodawania!";

            }
        }


        private bool IsFormValid()
        {
            return !(
                SelectedDayOfWeek == null ||
                SelectedGroup == null ||
                SelectedPlace == null ||
                SelectedSubject == null ||
                SelectedType == null ||
                StartTime == null ||
                EndTime == null ||
                EndTime <= StartTime

                );


        }

        private void Cancel()
        {
            SelectedGroup = null;
            SelectedDayOfWeek = null;
            SelectedPlace = null;
            SelectedSubject = null;
            SelectedType = null;
            StartTime = null;
            EndTime = null;
            ErrorString = "";
            SuccessString = "";
        }




        private bool IsValidTimeFormat(string? time)
        {
            if (time == null) return false;
            // Regex to match HH:MM format  
            return Regex.IsMatch(time, @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
        }





        private async Task LoadAsync<T>(ObservableCollection<T> TList) where T : class
        {
            List<T>? Ts = await RetrieveService.GetAllAsync<T>(wrapper);
            if (Ts == null) return;

            TList.Clear();
            foreach (T t in Ts) TList.Add(t);

        }

        public async Task LoadAll()
        {
            await LoadAsync<SubjectModel>(Subjects);
            await LoadAsync<PlaceModel>(Places);
            await LoadAsync<GroupEditModel>(Groups);

        }
        #endregion
    }

}
