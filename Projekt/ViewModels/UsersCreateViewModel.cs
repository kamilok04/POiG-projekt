using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class UsersCreateViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "UsersCreate";

        // for designer only
        public UsersCreateViewModel() { }


        #region Fields

        private string? _name;
        private string? _surname;
        private string? _login;
        private string? _password;
        private string? _email;
        private string? _currentRole;
        private DateTime? _birthDate;
        private int? _studentID;
        private string? _teacherTitle;
        private string? _position;
        private UsersCreateModel _usersCreateModel;
        private List<string> _roles = ["Administrator", "Pracownik", "Student"];
        //private ObservableCollection<Subject> _allSubjects;
        private bool _isStudentVisible = false;
        private bool _isTeacherVisible = false;


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
        public string? Surname
        {
            get => _surname;
            set
            {
                if (_surname != value)
                {
                    _surname = value;
                    OnPropertyChanged(nameof(Surname));
                }
            }
        }
        public string? Login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }
        public string? Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public int? StudentID
        {
            get => _studentID;
            set
            {
                if (_studentID != value)
                {
                    _studentID = value;
                    OnPropertyChanged(nameof(StudentID));
                }
            }
        }

        public string? Email { get => _email; set => _email = value; }
        public DateTime? BirthDate
        {
            get => _birthDate;
            set
            {
                if (_birthDate != value)
                {
                    _birthDate = value;
                    OnPropertyChanged(nameof(BirthDate));
                }
            }
        }


        public string? CurrentRole
        {
            get => _currentRole;
            set
            {
                if (_currentRole != value)
                {
                    _currentRole = value;
                    OnPropertyChanged(nameof(CurrentRole));
                    OnPropertyChanged(nameof(IsStudentVisible));
                    OnPropertyChanged(nameof(IsTeacherVisible));
                }
            }
        }

        public string? TeacherTitle { get => _teacherTitle; set => _teacherTitle = value; }

        public string? Position { get => _position; set => _position = value; }

        public UsersCreateModel UsersCreateModel { get => _usersCreateModel; set => _usersCreateModel = value; }
        public List<string> Roles { get => _roles; }


        //public ObservableCollection<Subject> AllSubjects { get => _allSubjects; set => _allSubjects = value; }

        //public ObservableCollection<Subject> SelectedSubjects => AllSubjects.Where(s => s.IsSelectedSubject);


        public bool IsStudentVisible => CurrentRole == "Student";
        public bool IsTeacherVisible => CurrentRole == "Pracownik";
        public UsersCreateViewModel(LoginWrapper loginWrapper)
        {
            UsersCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }
       

        private ICommand? _suggestLoginCommand;

        public ICommand SuggestLoginCommand
        {
            get
            {
                _suggestLoginCommand ??= new RelayCommand(
                    p => Login = SuggestLogin(),
                    p => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Surname));
                return _suggestLoginCommand;
            }
        }

        private ICommand? _suggestStudentIDCommand;
        public ICommand SuggestStudentIDCommand
        {
            get
            {
                return _suggestStudentIDCommand ??= new RelayCommand(
                async p => StudentID = await SuggestStudentID()
                );
            }
        }

        private ICommand? _randomPasswordCommand;

        public ICommand RandomPasswordCommand
        {
            get
            {
                return _randomPasswordCommand ??= new RelayCommand(
                    p => Password = GenerateRandomPassword(12),
                    p => true);
               
            }
        }



        //private ICommand? _saveCommand;

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

        #endregion

        #region Private Methods

        private async Task<int> SuggestStudentID()
        {
            return await UsersCreateModel.LoginWrapper.DBHandler.SuggestStudentID();
        }
        private string SuggestLogin()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname))
                return "";
            // Prosty algorytm do sugerowania loginu
            string suggestedLogin = $"{Name.ToLowerInvariant()}.{Surname.ToLowerInvariant()}".Replace(" ", "");
            return suggestedLogin;
        }

        private string GenerateRandomPassword(int length)
        {
           // Bezpieczny generator haseł
           return IHashingHandler.GetRandomString(length);

        }

        #endregion


    }
}
