using Projekt.Miscellaneous;
using Projekt.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

using System.Windows.Input;

namespace Projekt.ViewModels
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Backing fields use _ prefix by convention.")]
    public class UsersCreateViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "UsersCreate";

        #region Fields

        private int Permissions = 0;
        private UsersCreateModel Model;
        private readonly string[] _roles = ["Nadadministrator", "Administrator", "Pracownik", "Student"];

        private string? _errorString;
        private string? _successString;

        #endregion
        #region Constructors
        public UsersCreateViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }

        #endregion

        #region Public Properties/Commands

        public string? Name
        {
            get => Model?._name;
            set
            {
                if (Model != null && Model._name != value)
                {
                    Model._name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string? Surname
        {
            get => Model?._surname;
            set
            {
                if (Model != null && Model._surname != value)
                {
                    Model._surname = value;
                    OnPropertyChanged(nameof(Surname));
                }
            }
        }
        public string? Login
        {
            get => Model?._login;
            set
            {
                if (Model != null && Model._login != value)
                {
                    Model._login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }
        public string? Password
        {
            get => Model?._password;
            set
            {
                if (Model != null && Model._password != value)
                {
                    Model._password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public int StudentID
        {
            get => Model?._studentID ?? 0;
            set
            {
                if (Model != null && Model._studentID != value)
                {
                    Model._studentID = value;
                    OnPropertyChanged(nameof(StudentID));
                }
            }
        }

        public string? Email
        {
            get => Model?._email;
            set
            {
                if (Model != null && Model._email != value)
                {
                    Model._email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
        public DateTime? BirthDate
        {
            get => Model?._birthDate;
            set
            {
                if (Model != null && Model._birthDate != value)
                {
                    Model._birthDate = value;
                    OnPropertyChanged(nameof(BirthDate));
                }
            }
        }

        public string? CurrentRole
        {
            get => Model?._currentRole;
            set
            {
                if (Model != null && Model._currentRole != value)
                {
                    Model._currentRole = value;
                    UpdateUserPermissions();
                    OnPropertyChanged(nameof(CurrentRole));
                    OnPropertyChanged(nameof(IsStudentVisible));
                    OnPropertyChanged(nameof(IsTeacherVisible));
                }
            }
        }

        public string? TeacherTitle
        {
            get => Model?._teacherTitle;
            set
            {
                if (Model != null && Model._teacherTitle != value)
                {
                    Model._teacherTitle = value;
                    OnPropertyChanged(nameof(TeacherTitle));
                }
            }
        }

        public string? Position
        {
            get => Model?._position;
            set
            {
                if (Model != null && Model._position != value)
                {
                    Model._position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public UsersCreateModel? UsersCreateModel { get => Model; init => Model = value ?? throw new ArgumentNullException(nameof(value)); }
        public string[] Roles => _roles;

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

        public string? ErrorString
        {
            get => _errorString; set
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
            get => _successString; set
            {
                if (_successString != value)
                {
                    _successString = value;
                    OnPropertyChanged(nameof(SuccessString));
                }
            }
        }

        #endregion

        #region Private Methods

        private void Cancel()
        {
            Name = Surname = Password = Login = Email = TeacherTitle = string.Empty;
            BirthDate = null;
            StudentID = 0;
            Position = string.Empty;
            CurrentRole = null;
            SuccessString = null;
            ErrorString = null;
        }

        private bool IsStudentIDAllowed(string? studentID)
        {
            if (string.IsNullOrEmpty(studentID) || !int.TryParse(studentID, out _))
                return false;
            return true;
        }

        private async Task<int> SuggestStudentID()
        {
            return await (UsersCreateModel?.LoginWrapper?.DBHandler?.SuggestStudentID() ?? Task.FromResult(0));
        }
        private string SuggestLogin()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname))
                return "";
            // Simple algorithm for suggesting login
            string suggestedLogin = $"{Name.ToLowerInvariant()}.{Surname.ToLowerInvariant()}".Replace(" ", "");
            return suggestedLogin;
        }

        // Secure password generator
        private string GenerateRandomPassword(int length)
            => IHashingHandler.GetRandomString(length);

        private bool AreAllFieldsFilled()
        {
            if (string.IsNullOrEmpty(Email))
                return false;

            bool valid = !(
                string.IsNullOrEmpty(Login) ||
                string.IsNullOrEmpty(Name) ||
                string.IsNullOrEmpty(Surname) ||
                string.IsNullOrEmpty(Password) ||
                BirthDate == null ||
                BirthDate >= DateTime.Today ||
                Permissions == 0
                );

            if (IsStudentVisible)
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
                case "Nadadministrator":
                    Permissions = PermissionHelper.God;
                    break;
                default:
                    Permissions = PermissionHelper.Blocked;
                    break;
            }
            Model._permissions = Permissions;
        }

        private async Task<bool> AddUser()
        {
            if (!AreAllFieldsFilled() || Model == null)
                return false;

            if (!MailAddress.TryCreate(Email, out _)) return false; 

            int success = await Model.AddUser();

            switch (success)
            {
                case 0:
                default:
                    ErrorString = "Dodawanie nieudane :( Spróbuj ponownie";
                    SuccessString = "";
                    break;
                case 1:
                    ErrorString = "";
                    SuccessString = "Dodawanie zakończone pomyślnie!";
                    break;
                case -1:
                    ErrorString = "Użytkownik o takiej nazwie już istnieje.\r\nWybierz inną nazwę i spróbuj ponownie.";
                    SuccessString = "";
                    break;
            }
            return success == 1;
        }
        #endregion
    }
}
