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
    public class UsersCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string TableName => "users";

        public string? DefaultQuery => "INSERT INTO uzytkownik VALUES (@username, @name, @surname, @birth_date, @email, @salt, @password, @permissions);";

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
                    }
                ;
            }         
        }
        

        public string? _name;
        public string? _surname;
        public string? _login;
        public string? _password;
        public string? _email;
        public string? _currentRole;
        public DateTime? _birthDate;
        public int _permissions;
        public int _studentID;
        public string? _teacherTitle;
        public string? _position;
        private DatabaseHandler DatabaseHandler => LoginWrapper?.DBHandler ?? throw new InvalidOperationException("LoginWrapper or DBHandler is already null");

        private string? _salt;

        public async Task<bool> AddUser()
        {

            _salt = IHashingHandler.GetRandomString(20);

            // Transakcja: dodaj użytkownika i przypisz mu rolę
            var defaultQuery = ((ITable)this).DefaultQuery ?? throw new InvalidOperationException("DefaultQuery can't be null");
            var defaultParameters = ((ITable)this).DefaultParameters ?? throw new InvalidOperationException("DefaultParameters can't be null");

            MySqlCommand AddUserCommand = DatabaseHandler.CreateCommand(defaultQuery, defaultParameters);
            MySqlCommand AssignRoleCommand = CreateRoleCommand();

            return await DatabaseHandler.ExecuteInTransactionAsync(AddUserCommand, AssignRoleCommand);

        }
        private async Task<bool> EnsureUserIsUnique()
        {
            string query = "SELECT COUNT(*) FROM uzytkownik WHERE login = @login";
            Dictionary<string, object> parameters = new() { { "@login", _login ?? string.Empty } };
            List<Dictionary<string, object>> result = await DatabaseHandler.ExecuteQueryAsync(query, parameters);
            if (result.Count > 0 && result[0].TryGetValue("COUNT(*)", out object? value))
            {
                int count = Convert.ToInt32(value);
                return count == 0; // true if unique
            }
            return false; // not unique
        }

        private MySqlCommand CreateRoleCommand()
        {

            string Query = "";
            Dictionary<string, object>? Parameters = null;
            // TODO: Przebudować - tu powinno przypisywać się 1 LUB WIĘCEJ ról.
            switch (_currentRole)
            {
                case "Student":

                    Query = "INSERT INTO student (login, indeks) VALUES( @login, @studentID)";
                    Parameters = new() {
                        { "@login", _login ?? string.Empty },
                        { "@studentID", _studentID }}
                    ;
                    break;
                case "Pracownik":
                    Query = "INSERT INTO pracownik (login, tytul) VALUES (@login, @title)";
                    Parameters = new() {
                        { "@login", _login ?? string.Empty },
                        { "@title", _teacherTitle ?? string.Empty }}
                    ;
                    break;

                    // Nie ma tabeli admin na razie, jest niepotrzebna
                //case "Administrator":
                //    Query = "INSERT INTO admin (login) VALUES (@login)";
                //    Parameters = new() {
                //        { "@login", _login }
                //    }
                //    ;
                //    break;

            }
            return DatabaseHandler.CreateCommand(Query, Parameters);
        }
    }
}
