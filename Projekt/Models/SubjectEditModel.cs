using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;

namespace Projekt.Models
{
    public class SubjectEditModel(LoginWrapper loginWrapper) : BaseTableModel(loginWrapper)
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Literature { get; set; }
        public string? PassConditions { get; set; }
        public Int16 Credits { get; set; }
        public string? FacultyId { get; set; }

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public string TableName => "przedmiot";

        public string? DefaultQuery => null; // No default query needed for editing

        public Dictionary<string, object>? DefaultParameters => null;

        // Updates the subject by calling the UpdateSubject stored procedure
        public async Task<bool> UpdateSubject()
        {
            try
            {
                var updateSubjectQuery = "CALL UpdateSubject(@p_przedmiot_id, @p_kod, @p_nazwa, @p_opis, @p_literatura, @p_warunki, @p_punkty, @p_wydzial_nazwa_krotka)";

                var updateSubjectParams = new Dictionary<string, object>
                {
                    { "@p_przedmiot_id", Id },
                    { "@p_kod", Code ?? string.Empty },
                    { "@p_nazwa", Name ?? string.Empty },
                    { "@p_opis", Description ?? string.Empty },
                    { "@p_literatura", Literature ?? string.Empty },
                    { "@p_warunki", PassConditions ?? string.Empty },
                    { "@p_punkty", Credits },
                    { "@p_wydzial_nazwa_krotka", FacultyId ?? string.Empty }
                };

                var updateSubjectCommand = DatabaseHandler.CreateCommand(updateSubjectQuery, updateSubjectParams);

                return await DatabaseHandler.ExecuteInTransactionAsync(updateSubjectCommand);
            }
            catch (Exception)
            {
                return false; // Return false on any error
            }
        }
    }
}