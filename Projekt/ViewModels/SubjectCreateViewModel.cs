using Microsoft.VisualBasic.Logging;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Projekt.ViewModels
{
    public class SubjectCreateViewModel: ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "SubjectCreate";

        public SubjectCreateViewModel() { } //for designer only

        #region Fields

        private string? _name;
        private uint _ectsPoints;
        private string? _code;
        private string? _description;
        private string? _passingCriteria;
        private string? _literature;
        private SubjectCreateModel _subjectCreateModel;

        private string _errorString;
        private string _successString;
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
        public uint EctsPoints
        {
            get => _ectsPoints;
            set
            {
                if (_ectsPoints != value)
                {
                    _ectsPoints = value;
                    OnPropertyChanged(nameof(EctsPoints));
                }
            }
        }
        public string? Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }
        public string? Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public string? PassingCriteria
        {
            get => _passingCriteria;
            set
            {
                if (_passingCriteria != value)
                {
                    _passingCriteria = value;
                    OnPropertyChanged(nameof(PassingCriteria));
                }
            }
        }
        public string? Literature
        {
            get => _literature;
            set
            {
                if (_literature != value)
                {
                    _literature = value;
                    OnPropertyChanged(nameof(Literature));
                }
            }
        }

        public string ErrorString
        {
            get => _errorString; set
            {
                _errorString = value;
                OnPropertyChanged(nameof(ErrorString));
            }
        }
        public string SuccessString
        {
            get => _successString; set
            {
                _successString = value;
                OnPropertyChanged(nameof(SuccessString));
            }
        }

        public SubjectCreateModel SubjectCreateModel { get => _subjectCreateModel; set => _subjectCreateModel = value; }

        public SubjectCreateViewModel(LoginWrapper loginWrapper)
        {
            SubjectCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }

        private ICommand? _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(
                    async param => await AddSubject(),
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
            Name = null;
            EctsPoints = 0;
            Code = null;
            Description = null;
            PassingCriteria = null;
            Literature = null;
        }

        private bool AreAllFieldsFilled()
        {
            bool valid = !(
                String.IsNullOrEmpty(Code) ||
                String.IsNullOrEmpty(Name) ||
                EctsPoints == 0 ||
                String.IsNullOrEmpty(PassingCriteria) ||
                String.IsNullOrEmpty(Literature) ||
                String.IsNullOrEmpty(Description)
                );

            return valid;
        }

        private async Task<bool> AddSubject()
        {
            // TODO: jakieś ErrorText ni
            if (!AreAllFieldsFilled()) return false;
            bool success = await SubjectCreateModel.AddSubject();

            if (!success)
            {
                ErrorString = "Dodawanie nieudane! Sprbuj ponownie";
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
