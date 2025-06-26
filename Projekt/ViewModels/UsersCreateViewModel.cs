using Microsoft.Xaml.Behaviors.Media;
using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using Projekt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Projekt.ViewModels
{
    public class UsersCreateViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "UsersCreate";

        #region Fields

        private int Permissions = 0;
        private UsersCreateModel Model;
        private readonly string[] _roles = ["Administrator", "Pracownik", "Student"];
        //private ObservableCollection<Subject> _allSubjects;
        private bool _isStudentVisible = false;
        private bool _isTeacherVisible = false;
        private string _errorString;
        private string _successString;


        #endregion
        #region Constructors
        public UsersCreateViewModel(LoginWrapper loginWrapper)
        {
            UsersCreateModel = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }
        // for designer only
        public UsersCreateViewModel() { }
        #endregion

        #region Public Properties/Commands

        public string? Name
        {
            get => Model._name;
            set
            {
                if (Model._name != value)
                {
                    Model._name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string? Surname
        {
            get => Model._surname;
            set
            {
                if (Model._surname != value)
                {
                    Model._surname = value;
                    OnPropertyChanged(nameof(Surname));
                }
            }
        }
        public string? Login
        {
            get => Model._login;
            set
            {
                if (Model._login != value)
                {
                    Model._login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }
        public string? Password
        {
            get => Model._password;
            set
            {
                if (Model._password != value)
                {
                     Model._password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public int StudentID
        {
            get => Model._studentID;
            set
            {
                if (Model._studentID != value)
                {
                    Model._studentID = value;
                    OnPropertyChanged(nameof(StudentID));
                }
            }
        }

        public string? Email { get => Model._email; set => Model._email = value; }
        public DateTime? BirthDate
        {
            get => Model._birthDate;
            set
            {
                if (Model._birthDate != value)
                {
                    Model._birthDate = value;
                    OnPropertyChanged(nameof(BirthDate));
                }
            }
        }


        public string? CurrentRole
        {
            get => Model._currentRole;
            set
            {
                if (Model._currentRole != value)
                {
                    Model._currentRole = value;
                    UpdateUserPermissions();
                    OnPropertyChanged(nameof(CurrentRole));
                    OnPropertyChanged(nameof(IsStudentVisible));
                    OnPropertyChanged(nameof(IsTeacherVisible));
                }
            }
        }

        public string? TeacherTitle { get => Model._teacherTitle; set => Model._teacherTitle = value; }

        public string? Position { get => Model._position; set => Model._position = value; }

        public UsersCreateModel UsersCreateModel { get => Model; init => Model = value; }
        public string[] Roles { get => _roles; }


        //public ObservableCollection<Subject> AllSubjects { get => _allSubjects; set => _allSubjects = value; }

        //public ObservableCollection<Subject> SelectedSubjects => AllSubjects.Where(s => s.IsSelectedSubject);


        public bool IsStudentVisible => CurrentRole == "Student";
        public bool IsTeacherVisible => CurrentRole == "Pracownik";
  

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



        private ICommand? _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ??= new RelayCommand(
                    async param => await AddUser(),
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

        public string ErrorString { get => _errorString; set
            {
                _errorString = value;
                OnPropertyChanged(nameof(ErrorString));
            } }
        public string SuccessString { get => _successString; set
            {
                _successString = value;
                OnPropertyChanged(nameof(SuccessString));
            }
        }

        #endregion

        #region Private Methods

        private void Cancel()
        {
            Name = Surname = Password = Login = Email = TeacherTitle = String.Empty;
            BirthDate = null;
            
        }

      

        private bool IsStudentIDAllowed(string? studentID)
        {

            if (string.IsNullOrEmpty(studentID) || !int.TryParse(studentID, out _))
                return false;
            return true;
        }

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

        // Bezpieczny generator haseł
        private string GenerateRandomPassword(int length)
            => IHashingHandler.GetRandomString(length);

        private bool AreAllFieldsFilled()
        {
            // email
            try { MailAddress address = new(Email); }
            catch { return false; }

            bool valid =  !(
                String.IsNullOrEmpty(Login) ||
                String.IsNullOrEmpty(Name) ||
                String.IsNullOrEmpty(Surname) ||
                String.IsNullOrEmpty(Password) ||
                BirthDate == null ||
                BirthDate >= DateTime.Today ||
                Permissions == 0
                );

            if(IsStudentVisible)
                valid &= IsStudentIDAllowed(StudentID.ToString());

            return valid;


        }

        private void UpdateUserPermissions()
        {

            switch (CurrentRole)
            {
                case "Student":
                    Permissions = PermissionHelper.CombinePermissions(
                        PermissionHelper.CanSeeOwnProfile,
                        PermissionHelper.CanSeeOwnSchedule);
                    break;
                case "Pracownik":
                    Permissions = PermissionHelper.CombinePermissions(
                        PermissionHelper.CanSeeOwnProfile,
                        PermissionHelper.CanSeeOtherProfiles,
                        PermissionHelper.CanEditOwnProfile,
                        PermissionHelper.CanEditOwnSchedule);
                    break;
                case "Administrator":
                    Permissions = PermissionHelper.CombinePermissions(
                        PermissionHelper.CanModifyData,
                        PermissionHelper.CanCreateClasses,
                        PermissionHelper.CanManageGroups,
                        PermissionHelper.CanSeeOtherProfiles,
                        PermissionHelper.CanSeeOtherSchedules,
                        PermissionHelper.CanEditOtherProfiles,
                        PermissionHelper.CanEditOtherSchedules
                        );
                    break;
                default:
                    Permissions = PermissionHelper.Blocked;
                    break;
            }
        }

        private async Task<bool> AddUser()
        {
            // TODO: jakieś ErrorText ni
            if (!AreAllFieldsFilled()) return false;
            bool success =  await Model.AddUser();

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
