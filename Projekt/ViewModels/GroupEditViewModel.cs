using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class GroupEditViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(GroupEditViewModel);

        private GroupViewTableModel Model { get; init; }

        private DataTable? _groups;
        public DataTable? Groups
        {
            get => _groups;
            private set
            {
                _groups = value;
                OnPropertyChanged(nameof(Groups));
            }
        }

        private DataRowView? _selectedGroup;
        public DataRowView? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                _selectedGroup = value;
                OnPropertyChanged(nameof(SelectedGroup));
                // Załaduj dane wybranej grupy do edycji
                if (_selectedGroup != null)
                {
                    LoadSelectedGroup();
                }
            }
        }

        public GroupEditViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper);
            GroupEditModel = new GroupEditModel(loginWrapper);
            GetDataAsync().ConfigureAwait(false);
        }

        public GroupEditViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            if (Model?.LoginWrapper != null && Model?.DefaultQuery != null)
            {
                Groups = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(Model.DefaultQuery);
            }
        }

        #region Fields
        private int _groupId;
        private string? _number;
        private string? _faculty;
        private string? _degree;
        private string? _semester;
        private GroupEditModel? _GroupEditModel;
        #endregion

        private string? _errorString;
        private string? _successString;

        #region Public Properties/Commands
        public int GroupId
        {
            get => _groupId;
            set
            {
                if (_groupId != value)
                {
                    _groupId = value;
                    OnPropertyChanged(nameof(GroupId));
                }
            }
        }

        public string Number
        {
            get => _number ?? string.Empty;
            set
            {
                if (_number != value)
                {
                    _number = value;
                    OnPropertyChanged(nameof(Number));
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

        public string Degree
        {
            get => _degree ?? string.Empty;
            set
            {
                if (_degree != value)
                {
                    _degree = value;
                    OnPropertyChanged(nameof(Degree));
                }
            }
        }

        public string Semester
        {
            get => _semester ?? string.Empty;
            set
            {
                if (_semester != value)
                {
                    _semester = value;
                    OnPropertyChanged(nameof(Semester));
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

        public GroupEditModel? GroupEditModel { get => _GroupEditModel; set => _GroupEditModel = value; }

        private ICommand? _updateSelectedCommand;
        public ICommand UpdateSelectedCommand
        {
            get
            {
                return _updateSelectedCommand ??= new RelayCommand(
                    async param => await UpdateGroup(),
                    param => AreAllFieldsFilled() && SelectedGroup != null);
            }
        }

        #endregion

        #region Private Methods

        private void LoadSelectedGroup()
        {
            if (SelectedGroup != null)
            {
                GroupId = Convert.ToInt32(SelectedGroup["ID grupy"]);
                Number = SelectedGroup["Numer Grupy"]?.ToString() ?? string.Empty;
                Faculty = SelectedGroup["Wydział"]?.ToString() ?? string.Empty;
                Degree = SelectedGroup["Kierunek"]?.ToString() ?? string.Empty;
                Semester = SelectedGroup["Semestr"]?.ToString() ?? string.Empty;
            }
        }

        private void ClearFields()
        {
            Faculty = string.Empty;
            Degree = string.Empty;
            Semester = string.Empty;
            GroupId = 0;
            SelectedGroup = null;
        }

        private void ClearEndStrings()
        {
            ErrorString = null;
            SuccessString = null;
        }

        private bool AreAllFieldsFilled()
        {
            return GroupId > 0 &&
                   !string.IsNullOrEmpty(Faculty) &&
                   !string.IsNullOrEmpty(Degree) &&
                   !string.IsNullOrEmpty(Semester); 
        }

        private async Task<bool> UpdateGroup()
        {
            if (GroupEditModel == null)
                throw new InvalidOperationException("GroupEditModel is not initialized.");

            ClearEndStrings();

            GroupEditModel.GroupId = GroupId;
            GroupEditModel.CurrentFaculty = Faculty;
            GroupEditModel.GroupNumber = Number;
            GroupEditModel.CurrentDegree = Degree;
            GroupEditModel.CurrentSemester = Semester;

            bool success = await GroupEditModel.UpdateGroup();
            if (success)
            {
                MessageBox.Show("Grupa została zaktualizowana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                //SuccessString = "Grupę zaktualizowano z powodzeniem!";

                await GetDataAsync();
                ClearFields();
            }
            else
            {
                //SuccessString = null;
                //ErrorString = "Aktualizacja nieudana! Podano nieprawidłowe dane! Spróbuj ponownie. Sprawdź czy dany kierunek istnieje dla wybranego wydziału.";
                MessageBox.Show("Aktualizacja nieudana! Podano nieprawidłowe dane! Spróbuj ponownie. Sprawdź czy dany kierunek istnieje dla wybranego wydziału.", "Błąd aktualizacji", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return success;
        }

        #endregion
    }
}