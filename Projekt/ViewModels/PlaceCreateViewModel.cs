using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class PlaceCreateViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "CreatePlace";

        public PlaceCreateViewModel() { }

        #region Fields
        private string? _buildingCode;
        private List<string> _faculties = new List<string> { "MS", "MT", "AEI", "Ch"};
        private int _classNumber;
        private string? _address;
        private string? _capacity;
        private string? _currentFaculty;
        private PlaceCreateModel? _placeCreateModel;
        #endregion

        private string? _errorString;
        private string? _successString;

        #region Public Properties/Commands
        public string BuildingCode
        {
            get => _buildingCode ?? string.Empty;
            set
            {
                if (_buildingCode != value)
                {
                    _buildingCode = value;
                    OnPropertyChanged(nameof(_buildingCode));
                }
            }
        }

        public List<string> Faculties
        {
            get => _faculties;
        }

        public int ClassNumber
        {
            get => _classNumber;
            set
            {
                if (_classNumber != value)
                {
                    _classNumber = value;
                    OnPropertyChanged(nameof(_classNumber));
                }
            }
        }

        public string Address
        {
            get => _address ?? string.Empty;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(_address));
                }
            }
        }

        public string Capacity
        {
            get => _capacity ?? string.Empty;
            set
            {
                if (_capacity != value)
                {
                    _capacity = value;
                    OnPropertyChanged(nameof(_capacity));
                }
            }
        }

        public string CurrentFaculty
        {
            get => _currentFaculty ?? string.Empty;
            set
            {
                if (_currentFaculty != value)
                {
                    _currentFaculty = value;
                    OnPropertyChanged(nameof(CurrentFaculty));
                    OnPropertyChanged(nameof(IsMS));
                    OnPropertyChanged(nameof(IsMT));
                    OnPropertyChanged(nameof(IsCh));
                    OnPropertyChanged(nameof(IsAEI));
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

        public bool IsMS => _currentFaculty == "MS";
        public bool IsMT => _currentFaculty == "MT";
        public bool IsCh => _currentFaculty == "Ch";
        public bool IsAEI => _currentFaculty == "AEI";

        public PlaceCreateModel? PlaceCreateModel { get => _placeCreateModel; set => _placeCreateModel = value; }

        public PlaceCreateViewModel(LoginWrapper loginWrapper)
        {
            PlaceCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }

        private ICommand? _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(
                    async param => await AddPlace(),
                    param => AreAllFieldsFilled());
            }
        }

        private ICommand? _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                _cancelCommand ??= new RelayCommand(
                    p => Cancel(),
                    p => true);
                return _cancelCommand;
            }
        }

        #endregion

        #region Private Methods

        private void Cancel()
        {
            BuildingCode = string.Empty;
            ClassNumber = 0;
            Address = string.Empty;
            Capacity = string.Empty;
            CurrentFaculty = string.Empty;
            PlaceCreateModel = null;
        }

        private bool AreAllFieldsFilled()
        {
            return !string.IsNullOrEmpty(BuildingCode) &&
                   ClassNumber > 0 &&
                   !string.IsNullOrEmpty(Address) &&
                   !string.IsNullOrEmpty(Capacity) &&
                   !string.IsNullOrEmpty(CurrentFaculty);
        }

        private async Task<bool> AddPlace()
        {
            if (PlaceCreateModel == null)
                throw new InvalidOperationException("PlaceCreateModel is not initialized.");
            PlaceCreateModel.BuildingCode = BuildingCode;
            PlaceCreateModel.ClassNumber = ClassNumber;
            PlaceCreateModel.Address = Address;
            PlaceCreateModel.Capacity = Capacity;
            PlaceCreateModel.CurrentFaculty = CurrentFaculty;
            bool success = await PlaceCreateModel.AddPlace();
            if (success)
            {
                ErrorString = null;
                SuccessString = "Miejsce utworzono z powodzeniem!";
                Cancel();
            }
            else
            {
                SuccessString = null;
                ErrorString = "Dodawanie nieudane! Spróbuj ponownie.";
            }
            return success;
        }
        #endregion
    }
}