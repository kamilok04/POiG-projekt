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

        public PlaceEditViewModel(LoginWrapper loginWrapper)
        {

            Model = new(loginWrapper);
            GetDataAsync().ConfigureAwait(false); ;
        }

        public PlaceEditViewModel() { } //for designer only

        private async Task GetDataAsync()
        {
            if (Model?.LoginWrapper != null && Model?.DefaultQuery != null)
            {
                Places = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(Model.DefaultQuery);
            }
        }


        //string IPageViewModel.Name => "EditPlace";
        //private SubjectViewTableModel? Model { get; init; }
        //private DataTable? _places;

        //public DataTable? Places {
        //    get => _places;
        //    set
        //    {
        //        _places = value;
        //        OnPropertyChanged(nameof(_places));
        //    }
        //}

        //public PlaceEditViewModel(LoginWrapper loginWrapper) {
        //    Model = new(loginWrapper);
        //    GetDataAsync().ConfigureAwait(false);
        //}

        //public PlaceEditViewModel() { }

        //private async Task GetDataAsync()
        //{
        //    if (Model?.LoginWrapper != null && Model?.DefaultQuery != null)
        //    {
        //        Places = await Model.LoginWrapper.DBHandler.GenerateDatatableAsync(Model.DefaultQuery);
        //    }
        //}

        //#region Fields
        //private int _placeId;
        //private string? _buildingCode;
        //private List<string> _faculties = new List<string> { "MS", "MT", "AEI", "Ch" };
        //private int _classNumber;
        //private string? _address;
        //private string? _capacity;
        //private string? _currentFaculty;
        //private PlaceEditModel? _placeEditModel;
        //#endregion

        //private string? _errorString;
        //private string? _successString;

        //#region Public Properties/Commands
        //public int PlaceId
        //{
        //    get => _placeId;
        //    set
        //    {
        //        if (_placeId != value)
        //        {
        //            _placeId = value;
        //            OnPropertyChanged(nameof(PlaceId));
        //        }
        //    }
        //}

        //public string BuildingCode
        //{
        //    get => _buildingCode ?? string.Empty;
        //    set
        //    {
        //        if (_buildingCode != value)
        //        {
        //            _buildingCode = value;
        //            OnPropertyChanged(nameof(BuildingCode));
        //        }
        //    }
        //}

        //public List<string> Faculties
        //{
        //    get => _faculties;
        //}

        //public int ClassNumber
        //{
        //    get => _classNumber;
        //    set
        //    {
        //        if (_classNumber != value)
        //        {
        //            _classNumber = value;
        //            OnPropertyChanged(nameof(ClassNumber));
        //        }
        //    }
        //}

        //public string Address
        //{
        //    get => _address ?? string.Empty;
        //    set
        //    {
        //        if (_address != value)
        //        {
        //            _address = value;
        //            OnPropertyChanged(nameof(Address));
        //        }
        //    }
        //}

        //public string Capacity
        //{
        //    get => _capacity ?? string.Empty;
        //    set
        //    {
        //        if (_capacity != value)
        //        {
        //            _capacity = value;
        //            OnPropertyChanged(nameof(Capacity));
        //        }
        //    }
        //}

        //public string CurrentFaculty
        //{
        //    get => _currentFaculty ?? string.Empty;
        //    set
        //    {
        //        if (_currentFaculty != value)
        //        {
        //            _currentFaculty = value;
        //            OnPropertyChanged(nameof(CurrentFaculty));
        //            OnPropertyChanged(nameof(IsMS));
        //            OnPropertyChanged(nameof(IsMT));
        //            OnPropertyChanged(nameof(IsCh));
        //            OnPropertyChanged(nameof(IsAEI));
        //        }
        //    }
        //}

        //public string? ErrorString
        //{
        //    get => _errorString;
        //    set
        //    {
        //        if (_errorString != value)
        //        {
        //            _errorString = value;
        //            OnPropertyChanged(nameof(ErrorString));
        //        }
        //    }
        //}

        //public string? SuccessString
        //{
        //    get => _successString;
        //    set
        //    {
        //        if (_successString != value)
        //        {
        //            _successString = value;
        //            OnPropertyChanged(nameof(SuccessString));
        //        }
        //    }
        //}

        //public bool IsMS => _currentFaculty == "MS";
        //public bool IsMT => _currentFaculty == "MT";
        //public bool IsCh => _currentFaculty == "Ch";
        //public bool IsAEI => _currentFaculty == "AEI";

        //public PlaceEditModel? PlaceEditModel { get => _placeEditModel; set => _placeEditModel = value; }
        //internal RelayCommand? RefreshCommand { get; private set; }
        //private readonly LoginWrapper _loginWrapper;

        //private ICommand? _saveCommand;
        //public ICommand SaveCommand
        //{
        //    get
        //    {
        //        return _saveCommand ??= new RelayCommand(
        //            async param => await UpdatePlace(),
        //            param => AreAllFieldsFilled());
        //    }
        //}

        //private ICommand? _cancelCommand;
        //public ICommand CancelCommand
        //{
        //    get
        //    {
        //        _cancelCommand ??= new RelayCommand(
        //            p => Cancel(),
        //            p => true);
        //        return _cancelCommand;
        //    }
        //}

        //#endregion

        //#region Private Methods

        //private void Cancel()
        //{
        //    BuildingCode = string.Empty;
        //    ClassNumber = 0;
        //    Address = string.Empty;
        //    Capacity = string.Empty;
        //    CurrentFaculty = string.Empty;
        //    PlaceId = 0;
        //    PlaceEditModel = null;
        //}

        //private bool AreAllFieldsFilled()
        //{
        //    return PlaceId > 0 &&
        //           !string.IsNullOrEmpty(BuildingCode) &&
        //           ClassNumber > 0 &&
        //           !string.IsNullOrEmpty(Address) &&
        //           !string.IsNullOrEmpty(Capacity) &&
        //           !string.IsNullOrEmpty(CurrentFaculty);
        //}

        //private async Task<bool> UpdatePlace()
        //{
        //    if (PlaceEditModel == null)
        //        throw new InvalidOperationException("PlaceEditModel is not initialized.");

        //    PlaceEditModel.PlaceId = PlaceId;
        //    PlaceEditModel.BuildingCode = BuildingCode;
        //    PlaceEditModel.ClassNumber = ClassNumber;
        //    PlaceEditModel.Address = Address;
        //    PlaceEditModel.Capacity = Capacity;
        //    PlaceEditModel.CurrentFaculty = CurrentFaculty;

        //    bool success = await PlaceEditModel.UpdatePlace();
        //    if (success)
        //    {
        //        ErrorString = null;
        //        SuccessString = "Miejsce zaktualizowano z powodzeniem!";
        //        Cancel();
        //    }
        //    else
        //    {
        //        SuccessString = null;
        //        ErrorString = "Aktualizacja nieudana! Spróbuj ponownie.";
        //    }
        //    return success;
        //}

        //public async Task LoadPlace(int placeId)
        //{
        //    if (PlaceEditModel == null)
        //        throw new InvalidOperationException("PlaceEditModel is not initialized.");

        //    var placeData = await PlaceEditModel.GetPlaceById(placeId);
        //    if (placeData != null)
        //    {
        //        PlaceId = placeId;
        //        BuildingCode = placeData.ContainsKey("buildingCode") ? placeData["buildingCode"]?.ToString() ?? string.Empty : string.Empty;
        //        ClassNumber = placeData.ContainsKey("numer") ? Convert.ToInt32(placeData["numer"]) : 0;
        //        Address = placeData.ContainsKey("adres") ? placeData["adres"]?.ToString() ?? string.Empty : string.Empty;
        //        Capacity = placeData.ContainsKey("pojemnosc") ? placeData["pojemnosc"]?.ToString() ?? string.Empty : string.Empty;
        //        CurrentFaculty = placeData.ContainsKey("id_wydzialu") ? placeData["id_wydzialu"]?.ToString() ?? string.Empty : string.Empty;
        //    }
        //}
        //#endregion
    }
}