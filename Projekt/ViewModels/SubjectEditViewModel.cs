using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Projekt.Miscellaneous;
using Projekt.Models;

namespace Projekt.ViewModels
{
    public class SubjectEditViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => nameof(SubjectEditViewModel);

        private SubjectEditModel Model { get; init; }
        private LoginWrapper Wrapper { get; init; }


        public NotifyTaskCompletion<ObservableCollection<ExtendedSubjectModel>> NotifySubjects { get; private set; }
        private async Task<ObservableCollection<ExtendedSubjectModel>> GetSubjects()
        {
            var subjects = await RetrieveService.GetAllAsync<SubjectModel>(Wrapper);
            return subjects != null ? [.. Array.ConvertAll<SubjectModel, ExtendedSubjectModel>(subjects.ToArray(), a => new(a))] : [];
        }
        public NotifyTaskCompletion<ObservableCollection<FacultyModel>> NotifyFaculties { get; private set; }
        private async Task<ObservableCollection<FacultyModel>> GetFaculties()
        {
            if (Wrapper == null || Wrapper.DBHandler == null)
                throw new InvalidOperationException("LoginWrapper or DBHandler is not initialized.");
           List<FacultyModel> faculties = await RetrieveService.GetAllAsync<FacultyModel>(Wrapper);
            if (faculties == null) return [];
            return [.. faculties];
        }


        private ExtendedSubjectModel? _selectedSubject;
        public ExtendedSubjectModel? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged(nameof(SelectedSubject));
                // Załaduj dane wybranego miejsca do edycji
                if (_selectedSubject != null)
                {
                    LoadSelectedSubject();
                }
            }
        }

        public SubjectEditViewModel(LoginWrapper loginWrapper)
        {
            Wrapper = loginWrapper;
            Model = new(Wrapper);
            NotifySubjects = new(GetSubjects());
            NotifyFaculties = new(GetFaculties());

        }



        public SubjectEditViewModel() { } //for designer only


        #region Fields
        private int _subjectId;
        private string? _name;
        private FacultyModel? _faculty;
        private string? _code;
        private string? _description;
        private string? _literature;
        private string? _passingTerms;
        private Int16 _points;
        private ExtendedSubjectModel? _ExtendedSubjectModel;
        #endregion

        private string _errorString = "";
        private string _successString = "";

        #region Public Properties/Commands
        public int SubjectId
        {
            get => _subjectId;
            set
            {
                if (_subjectId != value)
                {
                    _subjectId = value;
                    OnPropertyChanged(nameof(SubjectId));
                }
            }
        }

        public string Name
        {
            get => _name ?? string.Empty;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public FacultyModel? Faculty
        {
            get => _faculty;
            set
            {
                if (_faculty != value)
                {
                    _faculty = value;
                    OnPropertyChanged(nameof(Faculty));
                }
            }
        }

        public string Code
        {
            get => _code ?? string.Empty;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    OnPropertyChanged(nameof(Code));
                }
            }
        }

        public string Description
        {
            get => _description ?? string.Empty;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string PassingTerms
        {
            get => _passingTerms ?? string.Empty;
            set
            {
                if (_passingTerms != value)
                {
                    _passingTerms = value;
                    OnPropertyChanged(nameof(PassingTerms));
                }
            }
        }

        public Int16 Points
        {
            get => _points;
            set
            {
                if (_points != value)
                {
                    _points = value;
                    OnPropertyChanged(nameof(Points));
                }
            }
        }

        public string Literature
        {
            get => _literature ?? string.Empty;
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
            get => _errorString;
            set
            {
                if (_errorString != value)
                {
                    _errorString = value;
                    _successString = "";
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
                    _errorString = "";
                    OnPropertyChanged(nameof(SuccessString));
                }
            }
        }


        private ICommand? _updateSelectedCommand;
        public ICommand UpdateSelectedCommand
        {
            get
            {
                return _updateSelectedCommand ??= new RelayCommand(
                    async param => await HandleUpdate(),
                    param => AreAllFieldsFilled() && SelectedSubject != null);
            }
        }

        private ICommand? _deleteSelectedCommand;
        public ICommand DeleteSelectedCommand
        {
            get
            {
                return _deleteSelectedCommand ??= new RelayCommand(
                    async param =>
                    {
                        if (SelectedSubject != null)
                        {
                            await HandleDelete();
                            ClearFields();

                        }
                    },
                    param => SelectedSubject != null);
                    
            }
        }

        private async Task LoadSelectedSubject()
        {
            if (SelectedSubject == null)
            {
                ClearFields();
                return;
            }

            SubjectId = SelectedSubject.Id;
            Name = SelectedSubject.Name;
            Faculty = await RetrieveService.GetAsync<FacultyModel>(Wrapper, SelectedSubject.FacultyId);
            Code = SelectedSubject.Code;
            Points = SelectedSubject.Credits;
            Description = SelectedSubject.Description;
            Literature = SelectedSubject.Literature;
            PassingTerms = SelectedSubject.PassConditions;

            if (NotifyFaculties?.Result != null && Faculty != null)
            {
                var matchingFaculty = NotifyFaculties.Result.FirstOrDefault(f => f.Id == Faculty.Id);
                if (matchingFaculty != null)
                {
                    Faculty = matchingFaculty;
                }
            }


            await SelectedSubject.GetDetails(Wrapper);

            // Zastąp czekajki normalnymi danymi
            Description = SelectedSubject.Description;
            Literature = SelectedSubject.Literature;
            PassingTerms = SelectedSubject.PassConditions;

        
        }
        

        private void ClearFields()
        {
            Points = 0;
            Faculty = null;
            Code = string.Empty;
            Name = string.Empty;
            Literature = string.Empty;
            Description = string.Empty;
            PassingTerms = string.Empty;
            SubjectId = 0;
            SelectedSubject = null;
        }

  

        private bool AreAllFieldsFilled()
        {
            return SubjectId > 0 &&
                   Faculty != null &&
                   !string.IsNullOrEmpty(Code) &&
                   Points >= 0;
                   // puste opisy, brak literatury i 0 punktów są ok(WF);
        }

        public async Task HandleUpdate()
        {
            bool success = await Model.HandleUpdate(SelectedSubject.DataId, SelectedSubject.LiteratureId,
                SelectedSubject.DescriptionId,
                SelectedSubject.PassConditionsId,
                SelectedSubject.FacultyId,
                Name,  Code, Points, Literature, Description, PassingTerms);
            if(success)
            {
                SuccessString = "Przedmiot został zaktualizowany pomyślnie.";
                ClearFields();
                NotifySubjects = new(GetSubjects());
            }
            else
            {
                ErrorString = "Wystąpił błąd podczas aktualizacji przedmiotu.";
                
            }

        }

        public async Task HandleDelete()
        {
            if (SelectedSubject == null) return;
            MessageBoxResult result = MessageBox.Show(
                        "Potwierdzenie spowoduje nieodwracalne usunięcie przedmiotu z bazy. Kontynuować?",
                        "Usuwanie przedmiotu",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result != MessageBoxResult.Yes) { return; }
            bool success = await Model.HandleDelete(SelectedSubject.Id);
            if(success)
            {
                SuccessString = "Przedmiot został usunięty pomyślnie.";
                ClearFields();
                NotifySubjects = new(GetSubjects());
            }
            else
            {
                ErrorString = "Wystąpił błąd podczas usuwania przedmiotu.";
            }
        }

        #endregion
    }
}
