using Projekt.Models;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class GroupEditViewModel : INotifyPropertyChanged, IPageViewModel
    {
        private readonly GroupEditModel _model;
        private readonly LoginWrapper _loginWrapper;
        private int _groupId;

        private string? _groupNumber;
        private string? _currentFaculty;
        private string? _currentDegree;
        private string? _currentSemester;
        private bool _isLoading;

        public string Name => "Edycja grupy";

        public ObservableCollection<string> Faculties { get; set; } = new();
        public ObservableCollection<string> Degrees { get; set; } = new();
        public ObservableCollection<int> Semesters { get; set; } = new() { 1, 2, 3, 4, 5, 6, 7 };

        public string? GroupNumber
        {
            get => _groupNumber;
            set
            {
                _groupNumber = value;
                _model.GroupNumber = value;
                OnPropertyChanged();
            }
        }

        public string? CurrentFaculty
        {
            get => _currentFaculty;
            set
            {
                _currentFaculty = value;
                _model._currentFaculty = value;
                OnPropertyChanged();
                LoadDegreesAsync();
            }
        }

        public string? CurrentDegree
        {
            get => _currentDegree;
            set
            {
                _currentDegree = value;
                _model._currentDegree = value;
                OnPropertyChanged();
            }
        }

        public string? CurrentSemester
        {
            get => _currentSemester;
            set
            {
                _currentSemester = value;
                _model._currentSemester = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? OnSaved;
        public event Action? OnCancelled;

        public GroupEditViewModel(LoginWrapper loginWrapper, int groupId)
        {
            _loginWrapper = loginWrapper;
            _groupId = groupId;
            _model = new GroupEditModel(loginWrapper);

            // Poprawione komendy - dodanie parametru object
            SaveCommand = new RelayCommand(async (parameter) => await SaveAsync(), (parameter) => CanSave());
            CancelCommand = new RelayCommand((parameter) => OnCancelled?.Invoke());

            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                // Load faculties first
                await LoadFacultiesAsync();

                // Load group data
                var success = await _model.LoadGroupData(_groupId);
                if (success)
                {
                    GroupNumber = _model.GroupNumber;
                    CurrentFaculty = _model._currentFaculty;
                    CurrentSemester = _model._currentSemester;

                    // Load degrees for the selected faculty
                    await LoadDegreesAsync();
                    CurrentDegree = _model._currentDegree;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadFacultiesAsync()
        {
            try
            {
                var query = "SELECT nazwa_krotka FROM wydzial ORDER BY nazwa_krotka";
                var result = await _loginWrapper.DBHandler.ExecuteQueryAsync(query);

                Faculties.Clear();
                if (result != null)
                {
                    foreach (var row in result)
                    {
                        if (row.ContainsKey("nazwa_krotka"))
                        {
                            Faculties.Add(row["nazwa_krotka"]?.ToString() ?? string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading faculties: {ex.Message}");
            }
        }

        private async Task LoadDegreesAsync()
        {
            if (string.IsNullOrEmpty(CurrentFaculty))
            {
                Degrees.Clear();
                return;
            }

            try
            {
                var query = @"SELECT DISTINCT dk.nazwa 
                             FROM dane_kierunku dk
                             JOIN kierunek k ON dk.id = k.id_danych_kierunku
                             JOIN wydzial w ON k.id_wydzialu = w.id
                             WHERE w.nazwa_krotka = @faculty
                             ORDER BY dk.nazwa";

                var parameters = new Dictionary<string, object>
                {
                    { "@faculty", CurrentFaculty }
                };

                var result = await _loginWrapper.DBHandler.ExecuteQueryAsync(query, parameters);

                Degrees.Clear();
                if (result != null)
                {
                    foreach (var row in result)
                    {
                        if (row.ContainsKey("nazwa"))
                        {
                            Degrees.Add(row["nazwa"]?.ToString() ?? string.Empty);
                        }
                    }
                }

                // Reset current degree if not available in new list
                if (!string.IsNullOrEmpty(CurrentDegree) && !Degrees.Contains(CurrentDegree))
                {
                    CurrentDegree = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading degrees: {ex.Message}");
            }
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(GroupNumber) &&
                   !string.IsNullOrWhiteSpace(CurrentFaculty) &&
                   !string.IsNullOrWhiteSpace(CurrentDegree) &&
                   !string.IsNullOrWhiteSpace(CurrentSemester) &&
                   !IsLoading;
        }

        private async Task SaveAsync()
        {
            try
            {
                IsLoading = true;
                var success = await _model.UpdateGroup();

                if (success)
                {
                    OnSaved?.Invoke();
                }
                else
                {
                    Console.WriteLine("Failed to update group");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving group: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}