using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class DegreeCreateViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "DegreeCreate";

        private DegreeCreateModel _model;

        private string? _errorString;
        private string? _successString;
        private LoginWrapper? _loginWrapper;

        public DegreeCreateViewModel() { _model = new(new(new(), "", "")); }

        public DegreeCreateViewModel(LoginWrapper loginWrapper)
        {
            _loginWrapper = loginWrapper;
            _model = new DegreeCreateModel(loginWrapper);

            _ = LoadFacultiesAsync();

        }

        #region Public Properties/Commands
        public string? Faculty
        {
            get => _model.Faculty;
            set
            {
                if (_model.Faculty != value)
                {
                    _model.Faculty = value;
                    OnPropertyChanged(nameof(Faculty));
                }
            }
        }
        public string? Name
        {
            get => _model.Name;
            set
            {
                if (_model.Name != value)
                {
                    _model.Name = value;
                    OnPropertyChanged(nameof(Name));
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

        public ObservableCollection<string> Faculties { get; set; } = new();

        public DegreeCreateModel DegreeCreateModel
        {
            get => _model;
            set
            {
                if (_model != value)
                {
                    _model = value;
                    OnPropertyChanged(nameof(DegreeCreateModel));
                }
            }
        }


        private ICommand? _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(
                    async param => await AddDegree(),
                    param => AreAllFieldsFilled());
            }
        }

        private ICommand? _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                _cancelCommand ??= new RelayCommand(
                    p => Cancel());
                return _cancelCommand;
            }
        }
        #endregion

        #region Private Helpers
        private void Cancel()
        {
            Name = Faculty = String.Empty;
        }

        private bool AreAllFieldsFilled()
        {
            bool valid = !(
                String.IsNullOrEmpty(Faculty) ||
                String.IsNullOrEmpty(Name)
                );

            return valid;
        }

        private async Task<bool> AddDegree()
        {
            if (!AreAllFieldsFilled()) return false;
            _model.Name = Name;
            _model.Faculty = Faculty;
            bool success = await DegreeCreateModel.AddDegree();

            if (!success)
            {
                ErrorString = "Dodawanie nieudane! Spróbuj ponownie";
                SuccessString = "";

            }
            else
            {
                ErrorString = "";
                SuccessString = "Dodano pomyślnie!";
            }
            return success;
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
        #endregion
    }
}
