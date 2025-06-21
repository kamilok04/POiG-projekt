using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class PlaceCreatorViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "CreatePlace";

        public PlaceCreatorViewModel() { }

        #region Fields
        private string? _buildingCode;
        private string? _faculty;
        private int _classNumber;
        private string? _address;
        private PlaceCreateModel _placeCreateModel;
        #endregion

        #region Public Properties/Commands
        public string BuildingCode
        {
            get => _buildingCode;
            set
            {
                if (_buildingCode != value)
                {
                    _buildingCode = value;
                    OnPropertyChanged(nameof(_buildingCode));
                }
            }
        }

        public string Faculty
        {
            get => _faculty;
            set
            {
                if (_faculty != value)
                {
                    _faculty = value;
                    OnPropertyChanged(nameof(_faculty));
                }
            }
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
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(_address));
                }
            }
        }

        public PlaceCreateModel PlaceCreateModel { get => _placeCreateModel; set => _placeCreateModel = value; }

        public PlaceCreatorViewModel(LoginWrapper loginWrapper)
        {
            PlaceCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }

        #endregion
    }
}
