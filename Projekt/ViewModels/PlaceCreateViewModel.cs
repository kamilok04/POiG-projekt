using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool IsMS => _currentFaculty == "MS";
        public bool IsMT => _currentFaculty == "MT";
        public bool IsCh => _currentFaculty == "Ch";
        public bool IsAEI => _currentFaculty == "AEI";

        public PlaceCreateModel? PlaceCreateModel { get => _placeCreateModel; set => _placeCreateModel = value; }

        public PlaceCreateViewModel(LoginWrapper loginWrapper)
        {
            PlaceCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }

        #endregion
    }
}
