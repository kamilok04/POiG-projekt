using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.ViewModels
{
    public class SubjectCreateViewModel: ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "SubjectCreate";

        public SubjectCreateViewModel() { } //for designer only

        #region Fields

        private string? _name;
        private string? _ectsPoints;
        private string? _code;
        private string? _description;
        private string? _passingCriteria;
        private string? _literature;
        private SubjectCreateModel? _subjectCreateModel;


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
        public string? EctsPoints
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

        public SubjectCreateModel? SubjectCreateModel { get => _subjectCreateModel; set => _subjectCreateModel = value; }

        public SubjectCreateViewModel(LoginWrapper loginWrapper)
        {
            SubjectCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }

        #endregion

        #region Private Methods

        #endregion


    }
}
