using Microsoft.VisualBasic.Logging;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using System.Xml.Linq;
using ZstdSharp.Unsafe;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.Design;

namespace Projekt.Models
{
    /// <summary>
    /// Model odpowiedzialny za tworzenie użytkowników w systemie.
    /// Pozwala na dodanie nowego użytkownika do bazy danych oraz przypisanie mu odpowiedniej roli.
    /// </summary>
    public class UsersCreateModel : BaseTableModel, ITable
    {
        /// <summary>
        /// Inicjalizuje nową instancję klasy <see cref="UsersCreateModel"/>.
        /// </summary>
        /// <param name="loginWrapper">Obiekt <see cref="LoginWrapper"/> zawierający informacje o aktualnym zalogowanym użytkowniku i połączeniu z bazą.</param>
        public UsersCreateModel(LoginWrapper loginWrapper) : base(loginWrapper) { }

        /// <summary>
        /// Nazwa tabeli w bazie danych, do której odnosi się model.
        /// </summary>
        public string TableName => "users";

        /// <summary>
        /// Domyślne zapytanie SQL do wstawiania nowego użytkownika.
        /// </summary>
        public string? DefaultQuery => "INSERT INTO uzytkownik VALUES (@username, @name, @surname, @birth_date, @email, @salt, @password, @permissions);";

        /// <summary>
        /// Domyślne parametry do zapytania SQL, generowane na podstawie pól modelu.
        /// </summary>
        public Dictionary<string, object>? DefaultParameters
        {
            get
            {
                var password = _password ?? string.Empty;
                var salt = _salt ?? string.Empty;

                return new(){
                        { "@username",_login ?? string.Empty },
                        { "@name", _name ?? string.Empty},
                        { "@surname", _surname ?? string.Empty},
                        { "@birth_date", _birthDate ?? DateTime.MinValue},
                        { "@email", _email ?? string.Empty},
                        { "@salt", _salt ?? string.Empty},
                        { "@password", IHashingHandler.GetHashString(password + salt) ?? string.Empty },
                        { "@permissions", _permissions }
                    };
            }
        }

        /// <summary>
        /// Imię użytkownika.
        /// </summary>
        public string? _name;
        /// <summary>
        /// Nazwisko użytkownika.
        /// </summary>
        public string? _surname;
        /// <summary>
        /// Login użytkownika.
        /// </summary>
        public string? _login;
        /// <summary>
        /// Hasło użytkownika (przed zahashowaniem).
        /// </summary>
        public string? _password;
        /// <summary>
        /// Adres e-mail użytkownika.
        /// </summary>
        public string? _email;
        /// <summary>
        /// Aktualnie wybrana rola użytkownika.
        /// </summary>
        public string? _currentRole;
        /// <summary>
        /// Data urodzenia użytkownika.
        /// </summary>
        public DateTime? _birthDate;
        /// <summary>
        /// Uprawnienia użytkownika (liczba całkowita).
        /// </summary>
        public int _permissions;
        /// <summary>
        /// Numer indeksu studenta (jeśli dotyczy).
        /// </summary>
        public int _studentID;
        /// <summary>
        /// Tytuł nauczyciela (jeśli dotyczy).
        /// </summary>
        public string? _teacherTitle;
        /// <summary>
        /// Stanowisko pracownika (jeśli dotyczy).
        /// </summary>
        public string? _position;

        /// <summary>
        /// Handler do obsługi bazy danych, pobierany z LoginWrapper.
        /// </summary>
        private DatabaseHandler DatabaseHandler => LoginWrapper?.DBHandler ?? throw new InvalidOperationException("LoginWrapper or DBHandler is already null");

        /// <summary>
        /// Sól używana do haszowania hasła.
        /// </summary>
        private string? _salt;

        /// <summary>
        /// Dodaje nowego użytkownika do bazy danych oraz przypisuje mu rolę.
        /// </summary>
        /// <returns>
        /// 1 jeśli operacja się powiodła, 0 jeśli nie, -1 jeśli użytkownik o podanym loginie już istnieje.
        /// </returns>
        public async Task<int> AddUser(string? role)
        {
            if (!await EnsureUserIsUnique()) return -1;

            _salt = IHashingHandler.GetRandomString(20);

            // Transakcja: dodaj użytkownika i przypisz mu rolę
            var defaultQuery = ((ITable)this).DefaultQuery ?? throw new InvalidOperationException("DefaultQuery can't be null");
            var defaultParameters = ((ITable)this).DefaultParameters ?? throw new InvalidOperationException("DefaultParameters can't be null");

            MySqlCommand AddUserCommand = DatabaseHandler.CreateCommand(defaultQuery, defaultParameters);
            MySqlCommand? AssignRoleCommand = CreateRoleCommand(role);
            bool transactionResult = false;

            if (AssignRoleCommand == null)
                transactionResult = await DatabaseHandler.ExecuteInTransactionAsync(AddUserCommand);
            else
                transactionResult = await DatabaseHandler.ExecuteInTransactionAsync(AddUserCommand, AssignRoleCommand);

            return transactionResult ? 1 : 0;
        }

        /// <summary>
        /// Sprawdza, czy login użytkownika jest unikalny w bazie danych.
        /// </summary>
        /// <returns>
        /// <c>true</c> jeśli login jest unikalny, <c>false</c> w przeciwnym wypadku.
        /// </returns>
        private async Task<bool> EnsureUserIsUnique()
        {
            string query = "SELECT COUNT(*) FROM uzytkownik WHERE login = @login";
            Dictionary<string, object> parameters = new() { { "@login", _login ?? string.Empty } };
            List<Dictionary<string, object>> result = await DatabaseHandler.ExecuteQueryAsync(query, parameters);
            if (result.Count > 0 && result[0].TryGetValue("COUNT(*)", out object? value))
            {
                int count = Convert.ToInt32(value);
                return count == 0; // true jeśli unikalny
            }
            return false; // nieunikalny
        }

        /// <summary>
        /// Tworzy polecenie SQL do przypisania roli użytkownikowi.
        /// </summary>
        /// <returns>
        /// Obiekt <see cref="MySqlCommand"/> do przypisania roli lub <c>null</c>, jeśli rola nie została wybrana.
        /// </returns>
        private MySqlCommand? CreateRoleCommand(string? role)
        {
            string Query = "";
            Dictionary<string, object>? Parameters = null;
            // -- zrobione -- TODO: Przebudować - tu powinno przypisywać się 1 LUB WIĘCEJ ról.
            // Więcej ról można przypisać w edytorze.
            switch (role)
            {
                case "Student":
                    Query = "INSERT INTO student (login, indeks) VALUES( @login, @studentID)";
                    Parameters = new() {
                            { "@login", _login ?? string.Empty },
                            { "@studentID", _studentID }
                        };
                    break;
                case "Pracownik":
                    Query = "INSERT INTO prowadzacy (login, tytul) VALUES (@login, @title)";
                    Parameters = new() {
                            { "@login", _login ?? string.Empty },
                            { "@title", _teacherTitle ?? string.Empty }
                        };
                    break;
                default: return null;
            }
            return DatabaseHandler.CreateCommand(Query, Parameters);
        }
    }
}
