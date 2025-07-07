using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;

namespace Projekt.Models
{
    /// <summary>
    /// Model służący do tworzenia nowego wydziału w bazie danych.
    /// </summary>
    public class FacultyCreateModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper), ITable
    {
        /// <summary>
        /// Skrócona nazwa wydziału.
        /// </summary>
        public string ShortName = "";

        /// <summary>
        /// Pełna nazwa wydziału.
        /// </summary>
        public string Name = "";

        /// <summary>
        /// Handler bazy danych powiązany z wrapperem logowania.
        /// </summary>
        private DatabaseHandler DatabaseHandler => LoginWrapper?.DBHandler
            ?? throw new InvalidOperationException("LoginWrapper or its DBHandler is null.");

        /// <summary>
        /// Nazwa tabeli w bazie danych.
        /// </summary>
        public string TableName => "wydzial";

        /// <summary>
        /// Domyślne zapytanie SQL do dodania wydziału.
        /// </summary>
        public string? DefaultQuery => "INSERT INTO wydzial (nazwa, nazwa_krotka) VALUES (@name, @shortName);";

        /// <summary>
        /// Domyślne parametry do zapytania SQL.
        /// </summary>
        public Dictionary<string, object>? DefaultParameters => new()
                    {
                        { "@name", Name },
                        { "@shortName", ShortName }
                    };

        /// <summary>
        /// Dodaje nowy wydział do bazy danych w ramach transakcji.
        /// </summary>
        /// <returns>True, jeśli operacja zakończyła się sukcesem; w przeciwnym razie false.</returns>
        public async Task<bool> AddFaculty()
        {
            // Ensure DefaultQuery is not null before proceeding
            if (string.IsNullOrEmpty(((ITable)this).DefaultQuery))
            {
                throw new InvalidOperationException("DefaultQuery cannot be null or empty.");
            }

            // Transakcja: dodaj wydział
            MySqlCommand AddFacultyCommand = DatabaseHandler.CreateCommand(((ITable)this).DefaultQuery, ((ITable)this).DefaultParameters);

            return await DatabaseHandler.ExecuteInTransactionAsync(AddFacultyCommand);
        }
    }
}
