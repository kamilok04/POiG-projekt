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

       
        private string _name { get; set; }
        private string _surname { get; set; }
        private string _birthDate { get; set; }
        private string _email { get; set; }
        private string _phone { get; set; }
        private string _studentID { get; set; }
        private string _title { get; set; }
        private bool _studentIDPresent = false;
        private bool _titlePresent = false;
        public string Name { get => _name; 
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
            }
        public string Surname { get => _surname; set
            {
                _surname = value;
                OnPropertyChanged(nameof(Surname));
            } }
        public string BirthDate { get => _birthDate; set
            {
                _birthDate = value;
                OnPropertyChanged(nameof(BirthDate));
            }}
        public string EMail { get => _email; set
            {
                _email = value;
                OnPropertyChanged(nameof(EMail));
            } }
        public string Phone { get=>_phone; set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
            } }
        public string StudentID { get=>_studentID; set
            {
                _studentID = value;
                OnPropertyChanged(nameof(StudentID));
            } }
        public string Title { get=>_title; set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            } }
        public bool StudentIDPresent
        {
            get => _studentIDPresent;
            set
            {
                _studentIDPresent = value;
                OnPropertyChanged(nameof(StudentIDPresent));
            }
        }

        public bool TitlePresent
        {
            get => _titlePresent;
            set
            {
                _titlePresent = value;
                OnPropertyChanged(nameof(TitlePresent));
            }
        }

        private LoginWrapper _loginWrapper { get; init; }

        public CurrentProfileViewModel(LoginWrapper loginWrapper)
        {
            _loginWrapper = loginWrapper;
            GetCurrentUserData().ConfigureAwait(false);
        }
        public CurrentProfileViewModel() { }
        private async Task GetCurrentUserData()
        {
            await _loginWrapper.Authenticate(PermissionHelper.CanSeeOwnProfile);
            var result = await _loginWrapper.DBHandler.ExecuteQueryAsync("SELECT imie, nazwisko, data_urodzenia, email, indeks, tytul FROM dane_uzytkownika WHERE login = @username",
                new Dictionary<string, object>() {
                    { "@username", _loginWrapper.Username }
                });
            if (result == null)
            {
                return;
            }
            var row = result[0];
            Name = (string) row["imie"];
            Surname = (string)row["nazwisko"];
            BirthDate = ((DateTime)row["data_urodzenia"]).ToShortDateString();
            EMail = (string)row["email"];
            int? tempID = (int?)row["indeks"];
            if (tempID.HasValue)
            {
                StudentID = tempID.Value.ToString();
            }
            Title = (string)row["tytul"];

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
    }
}
