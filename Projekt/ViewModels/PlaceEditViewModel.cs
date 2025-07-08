using Org.BouncyCastle.Utilities;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class PlaceEditViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(PlaceEditViewModel);

        private PlaceViewTableModel Model { get; init; }

        private DataTable? _places;
        public DataTable? Places
        {
            get => _places;
            private set
            {
                _places = value;
                OnPropertyChanged(nameof(Places));
            }
        }

        private DataRowView? _selectedPlace;
        public DataRowView? SelectedPlace
        {
            get => _selectedPlace;
            set
            {
                _selectedPlace = value;
                OnPropertyChanged(nameof(SelectedPlace));
                // Załaduj dane wybranego miejsca do edycji
                if (_selectedPlace != null)
                {
                    LoadSelectedPlace();
                }
            }
        }

        public PlaceEditViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper);
            PlaceEditModel = new PlaceEditModel(loginWrapper);
            GetDataAsync().ConfigureAwait(false);
        }

        public PlaceEditViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            if (Model?.LoginWrapper != null && Model?.DefaultQuery != null)
            {
                Places = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(Model.DefaultQuery);
            }
        }

        #region Fields
        private int _placeId;
        private string? _faculty;
        private string? _address;
        private int _classNumber;
        private string? _capacity;
        private PlaceEditModel? _placeEditModel;
        #endregion

        private string _errorString = "";
        private string _successString = "";


        #region Public Properties/Commands
        public int PlaceId
        {
            get => _placeId;
            set
            {
                if (_placeId != value)
                {
                    _placeId = value;
                    OnPropertyChanged(nameof(PlaceId));
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

        public string Address
        {
            get => _address ?? string.Empty;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
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
                    OnPropertyChanged(nameof(ClassNumber));
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
                    OnPropertyChanged(nameof(Capacity));
                }
            }
        }

        public string ErrorString
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

        public string SuccessString
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

        public PlaceEditModel? PlaceEditModel { get => _placeEditModel; set => _placeEditModel = value; }

        private ICommand? _updateSelectedCommand;
        public ICommand UpdateSelectedCommand
        {
            get
            {
                return _updateSelectedCommand ??= new RelayCommand(
                    async param => await UpdatePlace(),
                    param => AreAllFieldsFilled() && SelectedPlace != null);
            }
        }

        #endregion

        #region Private Methods

        private void LoadSelectedPlace()
        {
            if (SelectedPlace != null)
            {
                PlaceId = Convert.ToInt32(SelectedPlace["ID miejsca"]);
                Faculty = SelectedPlace["Wydział"]?.ToString() ?? string.Empty;
                Address = SelectedPlace["Adres"]?.ToString() ?? string.Empty;
                ClassNumber = Convert.ToInt32(SelectedPlace["Numer sali"]);
                Capacity = SelectedPlace["Pojemność"]?.ToString() ?? string.Empty;
            }
        }

        private void ClearFields()
        {
            ClassNumber = 0;
            Faculty = string.Empty;
            Address = string.Empty;
            Capacity = string.Empty;
            PlaceId = 0;
            SelectedPlace = null; 
        }

        private void ClearEndStrings()
        {
            ErrorString = "";
            SuccessString = "";
        }

        private bool AreAllFieldsFilled()
        {
            return PlaceId > 0 &&
                   ClassNumber > 0 &&
                   !string.IsNullOrEmpty(Faculty) &&
                   !string.IsNullOrEmpty(Address) &&
                   !string.IsNullOrEmpty(Capacity);
        }

        private async Task<bool> UpdatePlace()
        {
            if (PlaceEditModel == null)
                throw new InvalidOperationException("PlaceEditModel is not initialized.");

            ClearEndStrings();

            PlaceEditModel.PlaceId = PlaceId;
            PlaceEditModel.Faculty = Faculty;
            PlaceEditModel.ClassNumber = ClassNumber;
            PlaceEditModel.Address = Address;
            PlaceEditModel.Capacity = Capacity;

            bool success = await PlaceEditModel.UpdatePlace();
            if (success)
            {
                ErrorString = "";
                SuccessString = "Miejsce zaktualizowano z powodzeniem!";

                await GetDataAsync();
                ClearFields();
            }
            else
            {
                SuccessString = "";
                ErrorString = "Aktualizacja nieudana! Spróbuj ponownie.";
            }
            return success;
        }

        #endregion
    }
}