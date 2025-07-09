using Projekt.Miscellaneous;
using Projekt.Models;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;

using System.Windows.Input;

namespace Projekt.ViewModels
{
    /// <summary>
    /// ViewModel odpowiedzialny za logikę tworzenia nowego użytkownika w aplikacji.
    /// Udostępnia właściwości i komendy do powiązania z widokiem oraz obsługuje walidację i zapis danych.
    /// </summary>
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Backing fields use _ prefix by convention.")]
    public class UsersCreateViewModel : ObservableObject, IPageViewModel
    {
        /// <inheritdoc/>
        string IPageViewModel.Name => "UsersCreate";

        #region Fields

        /// <summary>
        /// Uprawnienia użytkownika.
        /// </summary>
        private int Permissions = 0;
        /// <summary>
        /// Model logiki tworzenia użytkownika.
        /// </summary>
        private UsersCreateModel Model;
        /// <summary>
        /// Lista dostępnych ról.
        /// </summary>
        private readonly string[] _roles = ["Nadadministrator", "Administrator", "Pracownik", "Student"];

        private string? _errorString;
        private string? _successString;

        #endregion

        #region Constructors
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="UsersCreateViewModel"/>.
        /// </summary>
        /// <param name="loginWrapper">Obiekt <see cref="LoginWrapper"/> z informacjami o sesji i połączeniu z bazą.</param>
        public UsersCreateViewModel(LoginWrapper loginWrapper)
        {
            Model = new(loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper)));
        }
        #endregion

        #region Public Properties/Commands

        /// <summary>
        /// Imię użytkownika.
        /// </summary>
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

        /// <summary>
        /// Nazwisko użytkownika.
        /// </summary>
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

        /// <summary>
        /// Login użytkownika.
        /// </summary>
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

        /// <summary>
        /// Hasło użytkownika.
        /// </summary>
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

        /// <summary>
        /// Numer indeksu studenta.
        /// </summary>
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

        /// <summary>
        /// Adres e-mail użytkownika.
        /// </summary>
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

        /// <summary>
        /// Data urodzenia użytkownika.
        /// </summary>
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

        /// <summary>
        /// Aktualnie wybrana rola użytkownika.
        /// </summary>
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

        /// <summary>
        /// Tytuł nauczyciela (jeśli dotyczy).
        /// </summary>
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

        /// <summary>
        /// Stanowisko pracownika (jeśli dotyczy).
        /// </summary>
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

        /// <summary>
        /// Model logiki tworzenia użytkownika (do powiązania z widokiem).
        /// </summary>
        public UsersCreateModel? UsersCreateModel { get => Model; init => Model = value ?? throw new ArgumentNullException(nameof(value)); }

        /// <summary>
        /// Lista dostępnych ról.
        /// </summary>
        public string[] Roles => _roles;

        /// <summary>
        /// Określa, czy widoczne są pola studenta.
        /// </summary>
        public bool IsStudentVisible => CurrentRole == "Student";
        /// <summary>
        /// Określa, czy widoczne są pola nauczyciela.
        /// </summary>
        public bool IsTeacherVisible => CurrentRole == "Pracownik";

        private ICommand? _suggestLoginCommand;
        /// <summary>
        /// Komenda sugerująca login na podstawie imienia i nazwiska.
        /// </summary>
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
        /// <summary>
        /// Komenda sugerująca numer indeksu studenta.
        /// </summary>
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
        /// <summary>
        /// Komenda generująca losowe hasło.
        /// </summary>
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
        /// <summary>
        /// Komenda zapisująca nowego użytkownika.
        /// </summary>
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
        /// <summary>
        /// Komenda anulująca wprowadzanie danych.
        /// </summary>
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

        /// <summary>
        /// Komunikat o błędzie.
        /// </summary>
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

        /// <summary>
        /// Komunikat o sukcesie.
        /// </summary>
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

        /// <summary>
        /// Czyści wszystkie pola formularza.
        /// </summary>
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

        /// <summary>
        /// Sprawdza, czy podany numer indeksu studenta jest poprawny.
        /// </summary>
        /// <param name="studentID">Numer indeksu jako string.</param>
        /// <returns><c>true</c> jeśli numer jest poprawny, w przeciwnym razie <c>false</c>.</returns>
        private bool IsStudentIDAllowed(string? studentID)
        {
            if (string.IsNullOrEmpty(studentID) || !int.TryParse(studentID, out _))
                return false;
            return true;
        }

        /// <summary>
        /// Asynchronicznie sugeruje nowy numer indeksu studenta.
        /// </summary>
        /// <returns>Nowy numer indeksu.</returns>
        private async Task<int> SuggestStudentID()
        {
            return await (UsersCreateModel?.LoginWrapper?.DBHandler?.SuggestStudentID() ?? Task.FromResult(0));
        }

        /// <summary>
        /// Sugeruje login na podstawie imienia i nazwiska.
        /// </summary>
        /// <returns>Sugerowany login.</returns>
        private string SuggestLogin()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Surname))
                return "";
            // Prosty algorytm sugerowania loginu
            string suggestedLogin = $"{Name.ToLowerInvariant()}.{Surname.ToLowerInvariant()}".Replace(" ", "");
            return suggestedLogin;
        }

        /// <summary>
        /// Generuje losowe hasło o zadanej długości.
        /// </summary>
        /// <param name="length">Długość hasła.</param>
        /// <returns>Wygenerowane hasło.</returns>
        private string GenerateRandomPassword(int length)
            => IHashingHandler.GetRandomString(length);

        /// <summary>
        /// Sprawdza, czy wszystkie wymagane pola są wypełnione.
        /// </summary>
        /// <returns><c>true</c> jeśli wszystkie pola są poprawnie wypełnione, w przeciwnym razie <c>false</c>.</returns>
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

        /// <summary>
        /// Aktualizuje uprawnienia użytkownika na podstawie wybranej roli.
        /// </summary>
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

        /// <summary>
        /// Asynchronicznie dodaje nowego użytkownika do bazy danych.
        /// </summary>
        /// <returns><c>true</c> jeśli dodanie się powiodło, w przeciwnym razie <c>false</c>.</returns>
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
