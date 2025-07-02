using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;

namespace Projekt.Models
{
    public class FacultyCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        public string ShortName = "";
        public string Name = "";
       

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public string TableName => "wydzial";

        // Czy to zapytanie jest poprawne?
        public string? DefaultQuery => "INSERT INTO wydzial (nazwa, nazwa_krotka) VALUES (@name, @shortName);";

        public Dictionary<string, object>? DefaultParameters => new()
            {

                { "@name", Name },
               { "@shortName", ShortName}
            };

        public async Task<bool> AddFaculty()
        {
            // Transakcja: dodaj przedmiot
            MySqlCommand AddFacultyCommand = DatabaseHandler.CreateCommand(((ITable)this).DefaultQuery, ((ITable)this).DefaultParameters);

            return await DatabaseHandler.ExecuteInTransactionAsync(AddFacultyCommand);
        }

    }
}
