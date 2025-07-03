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
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Projekt.ViewModels
{
    public class LessonsCreateViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "LessonsCreate";
        // for designer only
        public LessonsCreateViewModel() { }

        #region Fields

        private string? _selectedGroup;
        private string? _selectedSubject; 
        private string? _selectedPlace;
        private List<string> _daysOfWeek = new List<string> { "poniedziałek", "wtorek", "środa", "czwartek", "piątek", "sobota", "niedziela" };
        private string? _selectedDayOfWeek;
        private string? _startTime;
        private string? _endTime;
        private string? _errorString;
        private string? _successString;
        private LessonsCreateModel? _lessonsCreateModel;
        private LoginWrapper? _loginWrapper;

        #endregion

        #region Public Properties/Commands

        public List<string> Groups { get; set; } = new();
        public List<string> Subjects { get; set; } = new();
        public List<string> Places { get; set; } = new();

        public string? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup != value)
                {
                    _selectedGroup = value;
                    OnPropertyChanged(nameof(SelectedGroup));
                }
            }
        }

        public string? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (_selectedSubject != value)
                {
                    _selectedSubject = value;
                    OnPropertyChanged(nameof(SelectedSubject));
                }
            }
        }

        public string? SelectedPlace
        {
            get => _selectedPlace;
            set
            {
                if (_selectedPlace != value)
                {
                    _selectedPlace = value;
                    OnPropertyChanged(nameof(SelectedPlace));
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
                    LessonsCreateModel._dayOfWeek = value;
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
                    LessonsCreateModel._startTime = value;
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
                    LessonsCreateModel._endTime = value;
                    OnPropertyChanged(nameof(EndTime));
                }
            }
        }

        public string? ErrorString
        {
            get => _errorString;
            set
            {
                if (_errorString != value)
                {
                    _errorString = value;
                    OnPropertyChanged(nameof(ErrorString));
                }
            }
        }

        public string? SuccessString
        {
            get => _successString;
            set
            {
                if (_successString != value)
                {
                    _successString = value;
                    OnPropertyChanged(nameof(SuccessString));
                }
            }
        }

        public bool? IsEnabled { get => AreAllFieldsFilled(); }

        public LessonsCreateModel? LessonsCreateModel { get => _lessonsCreateModel; set => _lessonsCreateModel = value; }

        public LessonsCreateViewModel(LoginWrapper loginWrapper)
        {
            _loginWrapper = loginWrapper;
            LessonsCreateModel = new (loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));

            _ = LoadGroupsAsync();
            _ = LoadSubjectsAsync();
            _ = LoadPlacesAsync();
        }

        private ICommand? _saveCommand;
        public ICommand? SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(
                        async p => await AddLesson(),
                        p => AreAllFieldsFilled()
                    );
            }
        }

        private ICommand? _cancelCommand;
        public ICommand? CancelCommand
        {
            get
            {
                return _cancelCommand ??= new RelayCommand(
                    p => Cancel()
                    );
            }
        }

        #endregion

        #region Private Methods

        private bool IsValidTimeFormat(string time)
        {
            // Regex to match HH:MM format
            return Regex.IsMatch(time, @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$");
        }

        private async Task LoadGroupsAsync()
        {
            try
            {
                var query = @"
                SELECT 
                    grupa.numer, 
                    rocznik.semestr, 
                    dane_kierunku.nazwa, 
                    wydzial.nazwa_krotka
                FROM 
                    grupa
                JOIN 
                    rocznik ON grupa.id_rocznika = rocznik.id
                JOIN 
                    kierunek ON rocznik.id_kierunku = kierunek.id
                JOIN 
                    dane_kierunku ON kierunek.id_danych_kierunku = dane_kierunku.id
                JOIN 
                    wydzial ON kierunek.id_wydzialu = wydzial.nazwa_krotka;";

                var result = await _loginWrapper.DBHandler.ExecuteQueryAsync(query);

                Groups.Clear();
                if (result != null)
                {
                    foreach (var row in result)
                    {
                        Groups.Add($"nr: {row["numer"]?.ToString()}, sem: {row["semestr"]?.ToString()}, kierunek: {row["nazwa"]?.ToString()}, wydział: {row["nazwa_krotka"]}" ?? string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading faculties: {ex.Message}");
            }
        }

        private async Task LoadSubjectsAsync()
        {
            try
            {
                var query = @"
                    SELECT DISTINCT kod, nazwa
                    FROM dane_przedmiotu;";

                var result = await _loginWrapper.DBHandler.ExecuteQueryAsync(query);

                Subjects.Clear();
                if (result != null)
                {
                    foreach (var row in result)
                    {
                        Subjects.Add($"{row["kod"]?.ToString()}, {row["nazwa"]?.ToString()}" ?? string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading subjects: {ex.Message}");
            }
        }

        private async Task LoadPlacesAsync()
        {
            try
            {
                var query = @"
                    SELECT 
                        m.id_wydzialu AS wydzial, 
                        a.adres, 
                        m.numer, 
                        m.pojemnosc 
                    FROM 
                        miejsce m 
                    JOIN 
                        adres a 
                    ON 
                        m.id_adresu=a.id;";

                var result = await _loginWrapper.DBHandler.ExecuteQueryAsync(query);

                Places.Clear();
                if (result != null)
                {
                    foreach (var row in result)
                    {
                        Places.Add($"Wydział: {row["wydzial"]?.ToString()}, Adres: {row["adres"]?.ToString()}, Nr sali: {row["numer"]?.ToString()}, Pojemność: {row["pojemnosc"]?.ToString()}" ?? string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading subjects: {ex.Message}");
            }
        }

        private void Cancel()
        {
            SelectedDayOfWeek = null;
            SelectedGroup = null;
            SelectedPlace = null;
            SelectedSubject = null;
            StartTime = null;
            EndTime = null;
        }

        private bool AreAllFieldsFilled()
        {
            bool valid = !(
                String.IsNullOrEmpty(SelectedDayOfWeek) ||
                String.IsNullOrEmpty(SelectedGroup) ||
                String.IsNullOrEmpty(SelectedPlace) ||
                String.IsNullOrEmpty(SelectedSubject) ||
                String.IsNullOrEmpty(StartTime) ||
                String.IsNullOrEmpty(EndTime) //||
                //!IsValidTimeFormat(StartTime) ||
                //!IsValidTimeFormat(EndTime) 
                );

            return valid;
        }

        private async Task<bool> AddLesson()
        {
            if (!AreAllFieldsFilled() || LessonsCreateModel == null) return false;

            var groupIdQuery = @"
                SELECT grupa.id
                FROM grupa
                JOIN rocznik ON grupa.id_rocznika = rocznik.id
                JOIN kierunek ON rocznik.id_kierunku = kierunek.id
                JOIN dane_kierunku ON kierunek.id_danych_kierunku = dane_kierunku.id
                JOIN wydzial ON kierunek.id_wydzialu = wydzial.nazwa_krotka
                WHERE CONCAT('nr: ', grupa.numer, ', ', 'sem: ', rocznik.semestr, ', ', 'kierunek: ', dane_kierunku.nazwa, ', ', 'wydział: ', wydzial.nazwa_krotka)" +
                $" = '{SelectedGroup}';";

            var groupIdResult = await _loginWrapper.DBHandler.ExecuteQueryAsync(groupIdQuery);
            if (groupIdResult == null || groupIdResult.Count == 0)
            {
                ErrorString = "Nie znaleziono grupy. Sprawdź poprawność danych.";
                SuccessString = "";
                MessageBox.Show(ErrorString);
                return false;
            }

            var groupId = groupIdResult[0]["id"];
            LessonsCreateModel._groupId = Convert.ToInt32(groupId);

            var subjectIdQuery = @"
                SELECT p.id
                FROM przedmiot p
                JOIN dane_przedmiotu dp
                ON p.id_danych=dp.id " +
                $"WHERE concat(dp.kod,', ',dp.nazwa) = '{SelectedSubject}';";

            var subjectIdResult = await _loginWrapper.DBHandler.ExecuteQueryAsync(subjectIdQuery);
            if (subjectIdResult == null || subjectIdResult.Count == 0)
            {
                ErrorString = "Nie znaleziono przedmiotu. Sprawdź poprawność danych.";
                SuccessString = "";
                MessageBox.Show(ErrorString);
                return false;
            }
            var subjectId = subjectIdResult[0]["id"];
            LessonsCreateModel._subjectId = Convert.ToInt32(subjectId);

            var placeIdQuery = @"
                SELECT m.id
                FROM miejsce m
                JOIN adres a ON m.id_adresu = a.id
                WHERE CONCAT('Wydział: ', m.id_wydzialu, ', ', 'Adres: ', a.adres, ', ', 'Nr sali: ', m.numer, ', ', 'Pojemność: ', m.pojemnosc)" +
                $" = '{SelectedPlace}';";

            var placeIdResult = await _loginWrapper.DBHandler.ExecuteQueryAsync(placeIdQuery);
            if (placeIdResult == null || placeIdResult.Count == 0)
            {
                ErrorString = "Nie znaleziono miejsca. Sprawdź poprawność danych.";
                SuccessString = "";
                MessageBox.Show(ErrorString);
                return false;
            }
            var placeId = placeIdResult[0]["id"];
            LessonsCreateModel._placeId = Convert.ToInt32(placeId);

            MessageBox.Show($"Dodawanie zajęć: {LessonsCreateModel._dayOfWeek}, {LessonsCreateModel._startTime}, {LessonsCreateModel._endTime}, {LessonsCreateModel._subjectId}, {LessonsCreateModel._groupId}, {LessonsCreateModel._placeId}");

            bool success = await LessonsCreateModel.AddLesson();

            if (!success)
            {
                ErrorString = "Dodawanie nieudane! Spróbuj ponownie";
                SuccessString = "";
                MessageBox.Show(ErrorString);

            }
            else
            {
                ErrorString = "";
                SuccessString = "Dodano pomyślnie!";
                MessageBox.Show(SuccessString);
            }
            Cancel();
            

            return success;
        }

        #endregion
    }

}
