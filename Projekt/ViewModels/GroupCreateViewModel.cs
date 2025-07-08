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
using System.Windows;

namespace Projekt.ViewModels
{
    public class GroupCreateViewModel : ObservableObject, IPageViewModel
    {
        private readonly GroupCreateModel _model;
        private readonly LoginWrapper _loginWrapper;

        private string? _groupNumber;
        private string? _currentFaculty;
        private string? _currentDegree;
        private string? _currentSemester;
        private bool _isLoading;

        public string Name => "Tworzenie grupy";

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
                _ = LoadDegreesAsync();
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

        private ICommand? _saveCommand;
        public ICommand? SaveCommand 
        {
            get
            {
                return _saveCommand ??= new RelayCommand(
                    async param => await SaveAsync(),
                    param => CanSave());
            }
        }

        private ICommand? _cancelCommand;
        public ICommand CancelCommand { 
            get
            {
                return _cancelCommand ??= new RelayCommand(
                    param => Cancel());
            }
        }

        public event Action? OnSaved;
        public event Action? OnCancelled;

        public GroupCreateViewModel(LoginWrapper loginWrapper)
        {
            _loginWrapper = loginWrapper;
            _model = new GroupCreateModel(loginWrapper);

            _ = LoadFacultiesAsync();
        }

        private async Task LoadFacultiesAsync()
        {
            try
            {
                IsLoading = true;
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
            finally
            {
                IsLoading = false;
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
                IsLoading = true;
                var query = @"SELECT DISTINCT dk.nazwa 
                             FROM dane_kierunku dk
                             JOIN kierunek k ON dk.id = k.id_danych_kierunku
                             JOIN wydzial w ON k.id_wydzialu = w.nazwa_krotka
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
            finally
            {
                IsLoading = false;
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

        private bool IsFormValid()
        {
            return GroupNumber.Length > 0 && GroupNumber.Length <= 5;
        }

        private async Task SaveAsync()
        {
            try
            {
                IsLoading = true;

                if (!IsFormValid())
                {
                    MessageBox.Show("Proszę wprowadzić poprawne dane grupy. Numer grupy może mieć maksymalnie 5 znaków.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var success = await _model.AddGroup();

                if (success)
                {
                    OnSaved?.Invoke();
                    MessageBox.Show("Grupa została pomyślnie dodana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Optionally, you can reset the fields after saving
                    Cancel();
                }
                else
                {
                    // Handle error - you might want to show a message to the user
                    MessageBox.Show("Nie udało się dodać grupy. Spróbuj ponownie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void Cancel()
        {
            OnCancelled?.Invoke();
            // Optionally, you can reset the fields or perform other actions on cancel
            GroupNumber = null;
            CurrentFaculty = null;
            CurrentDegree = null;
            CurrentSemester = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

       

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}