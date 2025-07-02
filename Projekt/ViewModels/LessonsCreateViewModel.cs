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
        private List<string> _types = new List<string> { "Wykład", "Ćwiczenia", "Laboratoria", "Seminarium" };
        private string? _selectedType;
        //private ObservableCollection<Place>? _places;
        //private Place? _selectedTeacher;
        private List<string> _daysOfWeek = new List<string> { "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };
        private string? _selectedDayOfWeek;
        private string? _startTime;
        private string? _endTime;
        private LessonsCreateModel? _lessonsCreateModel;
        private LoginWrapper? _loginWrapper;

        #endregion

        #region Public Properties/Commands

        public List<string> Groups { get; set; } = new();
        public List<string> Subjects { get; set; } = new();
        public List<string> Places { get; set; } = new();

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

        public LessonsCreateModel? LessonsCreateModel { get => _lessonsCreateModel; set => _lessonsCreateModel = value; }

        public LessonsCreateViewModel(LoginWrapper loginWrapper)
        {
            _loginWrapper = loginWrapper;
            LessonsCreateModel = new (loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));

            _ = LoadGroupsAsync();
            _ = LoadSubjectsAsync();
            _ = LoadPlacesAsync();
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
                        Groups.Add($"nr: {row["numer"]?.ToString()}, sem: {row["semestr"]?.ToString()}, kierunek: {row["nazwa"]?.ToString()}, wydział: {row["nazwa_krotka"]} " ?? string.Empty);
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

                Groups.Clear();
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

                Groups.Clear();
                if (result != null)
                {
                    foreach (var row in result)
                    {
                        Places.Add($"Wydział: {row["wydzial"]?.ToString()}, \nAdres: {row["adres"]?.ToString()} \nNr sali: {row["numer"]?.ToString()}\nPojemność: {row["pojemnosc"]?.ToString()}" ?? string.Empty);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading subjects: {ex.Message}");
            }
        }

        #endregion
    }

}
