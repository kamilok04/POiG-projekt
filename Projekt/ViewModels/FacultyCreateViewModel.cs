using Projekt.Miscellaneous;
using Projekt.Models;
using System.Windows.Input;
using ZstdSharp.Unsafe;

namespace Projekt.ViewModels
{
    public class FacultyCreateViewModel: ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "FacultyCreate";

        public FacultyCreateViewModel() {  _model = new(new(new(), "", "")); } //forgive me

        #region Fields

        private string? _name;
        private string? _shortName;
        private FacultyCreateModel _model;

        private string? _errorString;
        private string? _successString;
        #endregion

        #region Public Properties/Commands
        public string? Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string? ShortName
        {
            get => _shortName;
            set
            {
                if (_shortName != value)
                {
                    _shortName = value;
                    OnPropertyChanged(nameof(ShortName));
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

        public FacultyCreateModel? FacultyCreateModel { get => _model; init => _model = value; }

        public FacultyCreateViewModel(LoginWrapper loginWrapper)
        {
            FacultyCreateModel = new(loginWrapper);
        }

        private ICommand? _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(
                    async param => await AddFaculty(),
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

        #region Private Methods

        private void Cancel()
        {
            Name = ShortName = String.Empty;
        }

        private bool AreAllFieldsFilled()
        {
            bool valid = !(
                String.IsNullOrEmpty(ShortName) ||
                String.IsNullOrEmpty(Name)
                
                );

            return valid;
        }

        private async Task<bool> AddFaculty()
        {
            // TODO: jakieś ErrorText ni
            if (!AreAllFieldsFilled()) return false;
            _model.Name = Name;
            _model.ShortName = ShortName;
            bool success = await _model.AddFaculty();

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
        #endregion


    }
}
