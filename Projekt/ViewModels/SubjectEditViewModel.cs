using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class SubjectEditViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(SubjectEditViewModel);

        private SubjectViewTableModel Model { get; init; }

        private DataTable? _subjects;
        public DataTable? Subjects
        {
            get => _subjects;
            private set
            {
                _subjects = value;
                OnPropertyChanged(nameof(Subjects));
            }
        }

        private DataRowView? _selectedSubject;
        public DataRowView? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged(nameof(SelectedSubject));
                // Załaduj dane wybranego miejsca do edycji
                if (_selectedSubject != null)
                {
                    LoadSelectedSubject();
                }
            }
        }

        public SubjectEditViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper);
            SubjectEditModel = new SubjectEditModel(loginWrapper);
            GetDataAsync().ConfigureAwait(false);
        }

        public SubjectEditViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            if (Model?.LoginWrapper != null && Model?.DefaultQuery != null)
            {
                Subjects = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(Model.DefaultQuery);
            }
        }

        #region Fields
        private int _subjectId;
        private string? _name;
        private string? _faculty;
        private string? _code;
        private string? _description;
        private string? _literature;
        private string? _passingTerms;
        private Int16 _points;
        private SubjectEditModel? _SubjectEditModel;
        #endregion

        private string? _errorString;
        private string? _successString;

        #region Public Properties/Commands
        public int SubjectId
        {
            get => _subjectId;
            set
            {
                if (_subjectId != value)
                {
                    _subjectId = value;
                    OnPropertyChanged(nameof(SubjectId));
                }
            }
        }

        public string Name
        {
            get => _name ?? string.Empty;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Faculty
        {
            get => _faculty ?? string.Empty;
            set
            {
                if (_faculty != value)
                {
                    _faculty = value;
                    OnPropertyChanged(nameof(Faculty));
                }
            }
        }

        public string Code
        {
            get => _code ?? string.Empty;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }

        public string Description
        {
            get => _description ?? string.Empty;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string PassingTerms
        {
            get => _passingTerms ?? string.Empty;
            set
            {
                if (_passingTerms != value)
                {
                    _passingTerms = value;
                    OnPropertyChanged(nameof(PassingTerms));
                }
            }
        }

        public Int16 Points
        {
            get => _points;
            set
            {
                if (_points != value)
                {
                    _points = value;
                    OnPropertyChanged(nameof(Points));
                }
            }
        }

        public string Literature
        {
            get => _literature ?? string.Empty;
            set
            {
                if (_literature != value)
                {
                    _literature = value;
                    OnPropertyChanged(nameof(Literature));
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

        public SubjectEditModel? SubjectEditModel { get => _SubjectEditModel; set => _SubjectEditModel = value; }

        private ICommand? _updateSelectedCommand;
        public ICommand UpdateSelectedCommand
        {
            get
            {
                return _updateSelectedCommand ??= new RelayCommand(
                    async param => await UpdatePlace(),
                    param => AreAllFieldsFilled() && SelectedSubject != null);
            }
        }

        #endregion

        #region Private Methods

        private void LoadSelectedSubject()
        {
            if (SelectedSubject != null)
            {
                SubjectId = Convert.ToInt32(SelectedSubject["ID przedmiotu"]);
                Name = SelectedSubject["Nazwa"]?.ToString() ?? string.Empty;
                Faculty = SelectedSubject["Wydział"]?.ToString() ?? string.Empty;
                Code = SelectedSubject["Kod"]?.ToString() ?? string.Empty;
                Description = SelectedSubject["Opis"]?.ToString() ?? string.Empty;
                Points = Convert.ToInt16(SelectedSubject["Punkty"]);
                Literature = SelectedSubject["Literatura"]?.ToString() ?? string.Empty;
                PassingTerms = SelectedSubject["Warunki zaliczenia"]?.ToString() ?? string.Empty;
            }
        }

        private void ClearFields()
        {
            Points = 0;
            Faculty = string.Empty;
            Code = string.Empty;
            Literature = string.Empty;
            Description = string.Empty;
            PassingTerms = string.Empty;
            SubjectId = 0;
            SelectedSubject = null;
        }

        private void ClearEndStrings()
        {
            ErrorString = null;
            SuccessString = null;
        }

        private bool AreAllFieldsFilled()
        {
            return SubjectId > 0 &&
                   !string.IsNullOrEmpty(Faculty) &&
                   !string.IsNullOrEmpty(Code) &&
                   !string.IsNullOrEmpty(Description) &&
                   Points > 0 &&
                   !string.IsNullOrEmpty(Literature);
        }

        private async Task<bool> UpdatePlace()
        {
            if (SubjectEditModel == null)
                throw new InvalidOperationException("SubjectEditModel is not initialized.");

            ClearEndStrings();

            SubjectEditModel.Id = SubjectId;
            SubjectEditModel.FacultyId = Faculty;
            SubjectEditModel.Name = Name;
            SubjectEditModel.Description = Description;
            SubjectEditModel.Code = Code;
            SubjectEditModel.Literature = Literature;
            SubjectEditModel.PassConditions = PassingTerms;
            SubjectEditModel.Credits = Points;

            bool success = await SubjectEditModel.UpdateSubject();
            if (success)
            {
                ErrorString = null;
                SuccessString = "Miejsce zaktualizowano z powodzeniem!";
                MessageBox.Show(SuccessString, "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);

                await GetDataAsync();
                ClearFields();
            }
            else
            {
                SuccessString = null;
                ErrorString = "Aktualizacja nieudana! Źle wprowadzone dane. Spróbuj ponownie. ";
                MessageBox.Show(ErrorString, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return success;
        }

        #endregion
    }
}
