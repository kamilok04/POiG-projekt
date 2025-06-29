using Projekt.Models;
using Projekt.Miscellaneous;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class GroupDeleteViewModel : ObservableObject, IPageViewModel
    {
        private readonly GroupDeleteModel _model;
        private readonly LoginWrapper _loginWrapper;
        private int _groupId;

        private string? _groupNumber;
        private string? _faculty;
        private string? _degree;
        private string? _semester;
        private int _studentCount;
        private bool _isLoading;
        private bool _canDelete;
        private string? _warningMessage;

        public string Name => "Usuwanie grupy";

        public string? GroupNumber
        {
            get => _groupNumber;
            set
            {
                _groupNumber = value;
                OnPropertyChanged();
            }
        }

        public string? Faculty
        {
            get => _faculty;
            set
            {
                _faculty = value;
                OnPropertyChanged();
            }
        }

        public string? Degree
        {
            get => _degree;
            set
            {
                _degree = value;
                OnPropertyChanged();
            }
        }

        public string? Semester
        {
            get => _semester;
            set
            {
                _semester = value;
                OnPropertyChanged();
            }
        }

        public int StudentCount
        {
            get => _studentCount;
            set
            {
                _studentCount = value;
                OnPropertyChanged();
                UpdateWarningMessage();
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

        public bool CanDelete
        {
            get => _canDelete;
            set
            {
                _canDelete = value;
                OnPropertyChanged();
            }
        }

        public string? WarningMessage
        {
            get => _warningMessage;
            set
            {
                _warningMessage = value;
                OnPropertyChanged();
            }
        }

        public string GroupInfo => $"Grupa: {GroupNumber} - {Faculty}, {Degree}, Semestr {Semester}";

        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action? OnDeleted;
        public event Action? OnCancelled;

        public GroupDeleteViewModel(LoginWrapper loginWrapper, int groupId)
        {
            _loginWrapper = loginWrapper;
            _groupId = groupId;
            _model = new GroupDeleteModel(loginWrapper);

            DeleteCommand = new RelayCommand(async (parameter) => await DeleteAsync(), (parameter) => CanDelete && !IsLoading);
            CancelCommand = new RelayCommand((parameter) => OnCancelled?.Invoke());

            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                var success = await _model.LoadGroupData(_groupId);
                if (success)
                {
                    GroupNumber = _model.GroupNumber;
                    Faculty = _model.Faculty;
                    Degree = _model.Degree;
                    Semester = _model.Semester;
                    StudentCount = _model.StudentCount;

                    CanDelete = await _model.CanDeleteGroup();
                    UpdateWarningMessage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading group data: {ex.Message}");
                WarningMessage = "Błąd podczas ładowania danych grupy.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateWarningMessage()
        {
            if (StudentCount > 0)
            {
                WarningMessage = $"UWAGA: Grupa zawiera {StudentCount} studentów. Nie można usunąć grupy z przypisanymi studentami.";
                CanDelete = false;
            }
            else
            {
                WarningMessage = "Czy na pewno chcesz usunąć tę grupę? Ta operacja jest nieodwracalna.";
                CanDelete = true;
            }
        }

        private async Task DeleteAsync()
        {
            try
            {
                IsLoading = true;

                var success = await _model.DeleteGroup();
                if (success)
                {
                    OnDeleted?.Invoke();
                }
                else
                {
                    WarningMessage = "Błąd podczas usuwania grupy.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting group: {ex.Message}");
                WarningMessage = "Błąd podczas usuwania grupy.";
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