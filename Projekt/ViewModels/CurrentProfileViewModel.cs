using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Projekt.ViewModels
{
    public class CurrentProfileViewModel : ObservableObject, IPageViewModel
    {
        string IPageViewModel.Name => "CurrentProfile";

       
        private string? _name { get; set; }
        private string? _surname { get; set; }
        private string? _birthDate { get; set; }
        private string? _email { get; set; }
        private string? _phone { get; set; }
        private string? _studentID { get; set; }
        private string? _title { get; set; }
        private bool? _studentIDPresent = false;
        private bool? _titlePresent = false;
        public string? Name { get => _name; 
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
            }
        public string? Surname { get => _surname; set
            {
                _surname = value;
                OnPropertyChanged(nameof(Surname));
            } }
        public string? BirthDate { get => _birthDate; set
            {
                _birthDate = value;
                OnPropertyChanged(nameof(BirthDate));
            }}
        public string? EMail { get => _email; set
            {
                _email = value;
                OnPropertyChanged(nameof(EMail));
            } }
        public string? Phone { get=>_phone; set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            } }
        public string? StudentID { get=>_studentID; set
            {
                _studentID = value;
                OnPropertyChanged(nameof(StudentID));
            } }
        public string? Title { get=>_title; set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            } }
        public bool? StudentIDPresent
        {
            get => _studentIDPresent;
            set
            {
                _studentIDPresent = value;
                OnPropertyChanged(nameof(StudentIDPresent));
            }
        }

        public bool? TitlePresent
        {
            get => _titlePresent;
            set
            {
                _titlePresent = value;
                OnPropertyChanged(nameof(TitlePresent));
            }
        }

        private LoginWrapper? _loginWrapper { get; init; }

        public CurrentProfileViewModel(LoginWrapper? loginWrapper)
        {
            _loginWrapper = loginWrapper ?? throw new ArgumentNullException(nameof(loginWrapper));
            _ = GetCurrentUserData();
        }
        public CurrentProfileViewModel() { }
        private async Task GetCurrentUserData()
        {
            if (_loginWrapper == null)
            {
                throw new ArgumentNullException(nameof(_loginWrapper));
            }
            try
            {
                await _loginWrapper.Authenticate(PermissionHelper.CanSeeOwnProfile);
                var result = await _loginWrapper.DBHandler.ExecuteQueryAsync("SELECT login Login, imie Imię, nazwisko Nazwisko, data_urodzenia 'Data Urodzenia', email 'Adres e-mail',uprawnienia Uprawnienia,indeks 'Nr indeksu',tytul 'Tytuł naukowy' FROM dane_uzytkownika WHERE login = @username",
                    new Dictionary<string, object>() {
                    { "@username", _loginWrapper.Username ?? string.Empty }
                    });
                if (result == null)
                {
                    return;
                }
                var row = result[0];
                Name = (string)row["Imię"];
                Surname = (string)row["Nazwisko"];
                BirthDate = ((DateTime)row["Data Urodzenia"]).ToShortDateString();
                EMail = (string)row["Adres e-mail"];
                int? tempID = (int?)row["Nr indeksu"];
                if (tempID.HasValue)
                {
                    StudentID = tempID.Value.ToString();
                }
                Title = (string)row["Tytuł naukowy"];

                if (!String.IsNullOrEmpty(StudentID))
                {
                    StudentIDPresent = true;
                }
                if (!String.IsNullOrEmpty(Title))
                {
                    TitlePresent = true;
                }

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving user data: {ex.Message}");
            }
        }
    }
}
