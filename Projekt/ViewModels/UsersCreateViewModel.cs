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

        private string? _name;
        private string? _surname;
        private string? _login;
        private string? _password;
        private string? _email;
        private string? _currentRole;
        private DateTime? _birthDate;
        private string? _teacherTitle;
        private string? _position;
        private UsersCreateModel _usersCreateModel;
        private List<string> _roles = new List<string> { "Administrator", "Pracownik", "Student" };
        //private ObservableCollection<Subject> _allSubjects;
        private bool _isStudentVisible = false;
        private bool _isTeacherVisible = false;
        #region Fields

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
        #endregion

        #region Public Properties/Commands
        public UsersCreateViewModel(LoginWrapper loginWrapper)
        {
            UsersCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));

        }
        // for designer only
        public UsersCreateViewModel() { }


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

        private ICommand? _randomPasswordCommand;

        public ICommand RandomPasswordCommand
        {
            get
            {
                _randomPasswordCommand ??= new RelayCommand(
                    p => Password = GenerateRandomPassword(12),
                    p => true);
                return _randomPasswordCommand;
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
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+[]{}|;:,.<>?";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #endregion


    }
}
