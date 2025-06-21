using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class GroupCreateViewModel: ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "GroupCreate";

        public GroupCreateViewModel() { }

        #region Fields
        private List<string> _faculties;
        private List<string> _degree;
        private List<int> _semesters = new List<int> { 1,2,3,4,5,6,7 };
        private string _currentFaculty;
        private string _currentDegree;
        private string _currentSemester;
        private GroupCreateModel _model;
        #endregion

        #region Public Properties/Commands
        public List<string> Faculties { get => _faculties; }
        public List<string> Degrees { get => _degree; }
        public List<int> Semesters {  get => _semesters; }

        public string CurrentFaculty 
        { 
            get => _currentFaculty; 
            set 
            {
                if (_currentFaculty != value)
                {
                    value = _currentFaculty;
                    OnPropertyChanged(CurrentFaculty);
                }
            } 
        }

        public string CurrentDegree
        {
            get => _currentDegree;
            set
            {
                if (_currentDegree != value)
                {
                    _currentDegree = value;
                    OnPropertyChanged(CurrentDegree);
                }
            }
        }

        public string CurrentSemester
        {
            get => _currentSemester;
            set
            {
                if (_currentSemester != value)
                {
                    _currentSemester = value;
                    OnPropertyChanged(CurrentSemester);
                }
            }
        }

        public GroupCreateModel Model { get => _model; set => _model = value; }

        public GroupCreateViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }

        #endregion
    }
}
