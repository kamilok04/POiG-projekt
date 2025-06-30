using MySql.Data.MySqlClient;
using Projekt.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt.Models
{
    public class PlaceCreateModel : BaseTableModel, ITable
    {
        public string? BuildingCode { get; set; }
        public List<string> Faculties = new List<string> { "MS", "MT", "AEI", "Ch" };
        public int ClassNumber { get; set; }
        public string? Address { get; set; }
        public string? Capacity { get; set; }
        public string? CurrentFaculty { get; set; }

        public string TableName => "miejsce";

        public string? DefaultQuery => BuildDefaultQuery();

        public Dictionary<string, object>? DefaultParameters => new()
        {
            { "@buildingCode", BuildingCode ?? string.Empty },
            { "@classNumber", ClassNumber },
            { "@address", Address ?? string.Empty },
            { "@capacity", int.TryParse(Capacity, out int cap) ? cap : 0 },
            { "@currentFaculty", CurrentFaculty ?? string.Empty }
        };

        private DatabaseHandler DatabaseHandler => LoginWrapper.DBHandler;

        public PlaceCreateModel(LoginWrapper loginWrapper) : base(loginWrapper)
        {
        }

        public async Task<bool> AddPlace()
        {
            try
            {
                var checkFacultyQuery = "SELECT COUNT(*) as count FROM wydzial WHERE nazwa_krotka = @currentFaculty";
                var checkFacultyParams = new Dictionary<string, object>
                {
                    { "@currentFaculty", CurrentFaculty ?? string.Empty }
                };

                var facultyResult = await DatabaseHandler.ExecuteQueryAsync(checkFacultyQuery, checkFacultyParams);
                int facultyCount = 0;
                if (facultyResult?.Count > 0 && facultyResult[0].ContainsKey("count"))
                {
                    facultyCount = Convert.ToInt32(facultyResult[0]["count"]);
                }

                var insertAddressQuery = "INSERT INTO adres (adres) VALUES (@address); SELECT LAST_INSERT_ID() as id;";
                var insertAddressParams = new Dictionary<string, object>
                {
                    { "@address", Address ?? string.Empty }
                };

                var addressResult = await DatabaseHandler.ExecuteQueryAsync(insertAddressQuery, insertAddressParams);
                int addressId = 0;
                if (addressResult?.Count > 0 && addressResult[0].ContainsKey("id"))
                {
                    addressId = Convert.ToInt32(addressResult[0]["id"]);
                }

                if (addressId == 0)
                {
                    return false;
                }

                if (facultyCount == 0)
                {
                    var insertFacultyQuery = "INSERT INTO wydzial (nazwa, nazwa_krotka) VALUES (@facultyName, @currentFaculty)";
                    var insertFacultyParams = new Dictionary<string, object>
                    {
                        { "@facultyName", GetFullFacultyName(CurrentFaculty ?? string.Empty) },
                        { "@currentFaculty", CurrentFaculty ?? string.Empty }
                    };

                    var insertFacultyCommand = DatabaseHandler.CreateCommand(insertFacultyQuery, insertFacultyParams);
                    var facultySuccess = await DatabaseHandler.ExecuteInTransactionAsync(insertFacultyCommand);

                    if (!facultySuccess)
                    {
                        return false;
                    }
                }

                var insertPlaceQuery = "INSERT INTO miejsce (id_wydzialu, id_adresu, numer, pojemnosc) VALUES (@currentFaculty, @addressId, @classNumber, @capacity)";
                var insertPlaceParams = new Dictionary<string, object>
                {
                    { "@currentFaculty", CurrentFaculty ?? string.Empty },
                    { "@addressId", addressId },
                    { "@classNumber", ClassNumber },
                    { "@capacity", int.TryParse(Capacity, out int cap) ? cap : 0 }
                };

                var insertPlaceCommand = DatabaseHandler.CreateCommand(insertPlaceQuery, insertPlaceParams);
                return await DatabaseHandler.ExecuteInTransactionAsync(insertPlaceCommand);
            }
            catch (Exception)
            {
                throw new Exception("Query failed to execute. Please check the parameters and connection.");
            }
        }

        private string BuildDefaultQuery()
        {
            return @"
                INSERT INTO adres (adres) VALUES (@address);
                SET @address_id = LAST_INSERT_ID();
                INSERT INTO miejsce (id_wydzialu, id_adresu, numer, pojemnosc) 
                VALUES (@currentFaculty, @address_id, @classNumber, @capacity);";
        }

        private string GetFullFacultyName(string shortName)
        {
            return shortName switch
            {
                "MS" => "Matematyki i Statystyki",
                "MT" => "Mechaniki i Technologii",
                "AEI" => "Automatyki, Elektroniki i Informatyki",
                "Ch" => "Chemii",
                _ => shortName
            };
        }
    }
}